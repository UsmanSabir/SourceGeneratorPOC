using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace GeneratorLib
{
    public static class Scanner
    {
        //private const string FullyQualifiedMetadataName = "Microsoft.AspNetCore.Mvc.ControllerBase";
        private const string FullyQualifiedMetadataName = "CoreLib.IBusinessService";
        //
        public static IEnumerable<ControllerRoute> ScanForControllers(SemanticModel semantic)
        {
            //IEnumerable<SyntaxNode> allNodes = semantic.Compilation.SyntaxTrees
            //    .SelectMany(s => s.GetRoot().DescendantNodes());
            //IEnumerable<ClassDeclarationSyntax> allClasses = allNodes
            //    .Where(d => d.IsKind(SyntaxKind.ClassDeclaration))
            //    .OfType<ClassDeclarationSyntax>();

            //allClasses
            //    .Select(component => GetRoutePath(compilation, component))
            //    .Where(route => route is not null)
            //    .Cast<string>()// stops the nullable lies
            //    .ToImmutableArray();

            var controllerBase = semantic.Compilation.GetTypeByMetadataName(FullyQualifiedMetadataName);
            //File.WriteAllText("C:\\Temp\\Base.txt", controllerBase?.Name ?? "NULL");
            if (controllerBase == null)
            {
                yield break;
            }

            var allNodes = semantic.SyntaxTree.GetRoot().DescendantNodesAndSelf() //.DescendantNodes()
                .OfType<ClassDeclarationSyntax>();

            foreach (var node in allNodes)
            {
                if (semantic.GetDeclaredSymbol(node) is INamedTypeSymbol classSymbol && InheritsFrom(classSymbol, controllerBase))
                {
                    File.WriteAllText($"C:\\Temp\\Imp{classSymbol.Name}.txt", classSymbol?.Name ?? "NULL");
                    yield return ToRoute(classSymbol);
                }
            }
        }

        private static ControllerRoute ToRoute(INamedTypeSymbol classSymbol)
        {
            const string suffix = "Service";// "Controller";
            var name = classSymbol.Name.EndsWith(suffix)
                ? classSymbol.Name.Substring(0, classSymbol.Name.Length - suffix.Length)
                : classSymbol.Name;

            var actionMethods = ScanForActionMethods(classSymbol)
                .ToArray();

            // Extract the route from the HttpActionAttribute
            var attribute = FindAttribute(classSymbol, a => a.ToString() == "Microsoft.AspNetCore.Mvc.RouteAttribute");
            var route = attribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;

            var areaAttribute = FindAttribute(classSymbol, a => a.ToString() == "Microsoft.AspNetCore.Mvc.AreaAttribute");
            var area = areaAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? null;

            return new ControllerRoute(name, area, route, actionMethods);
        }

        private static IEnumerable<ActionRoute> ScanForActionMethods(INamedTypeSymbol classSymbol)
        {
            //Debugger.Launch();
            var allInterfaceMethods = classSymbol.AllInterfaces
       .SelectMany(x => x.GetMembers())
       //.Concat(targetType.GetMembers())
       .OfType<IMethodSymbol>()
       .ToList();

            var methodSignatures = allInterfaceMethods.Select(x => GetFullQualifiedName(x.ReturnType) + " " +
            x.Name + "(" +
            String.Join(", ", x.Parameters.Select(p => GetFullQualifiedName(p.Type) + " " + p.Name))
            + ")"
            ).ToList();
            StringBuilder sb=new StringBuilder();
            foreach (var methodSignature in methodSignatures) sb.AppendLine(methodSignature);
            File.WriteAllText($"C:\\Temp\\Sig-{classSymbol.Name}.txt", sb.ToString());

            foreach (var member in classSymbol.GetMembers())
            {
                if (member
                    is IMethodSymbol { DeclaredAccessibility: Accessibility.Public, IsAbstract: false } methodSymbol
                    and not { MethodKind: MethodKind.Constructor }
                   )
                {
                    if (methodSymbol.Name.StartsWith("get_") || methodSymbol.Name.StartsWith("set_"))
                    {
                        continue;
                    }

                    var name = methodSymbol.Name;
                    var returnType = methodSymbol.ReturnType;

                    // Unwrap Task<T>
                    if (returnType is INamedTypeSymbol taskType && taskType.OriginalDefinition.ToString() == "System.Threading.Tasks.Task<TResult>")
                    {
                        returnType = taskType.TypeArguments.First();
                    }

                    // Take unwrapped T and check whether we need to 
                    // unwrap further to V when T = ActionResult<V>
                    if (returnType is INamedTypeSymbol actionResultType && actionResultType.OriginalDefinition.ToString() == "Microsoft.AspNetCore.Mvc.ActionResult<TValue>")
                    {
                        returnType = actionResultType.TypeArguments.First();
                    }

                    // If the return type is simple IActionResult -- assume that the return type is essentially void
                    if (returnType.OriginalDefinition.ToString() == "Microsoft.AspNetCore.Mvc.IActionResult" || returnType.OriginalDefinition.ToString() == "Microsoft.AspNetCore.Mvc.ActionResult")
                    {
                        returnType = null;
                    }

                    // Extract the route from the HttpActionAttribute
                    var attribute = FindAttribute(methodSymbol, a => a.BaseType?.ToString() == "Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute");
                    var route = attribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
                    var method = attribute?.AttributeClass?.Name switch
                    {
                        "HttpGetAttribute" => HttpMethod.Get,
                        "HttpPutAttribute" => HttpMethod.Put,
                        "HttpPostAttribute" => HttpMethod.Post,
                        "HttpDeleteAttribute" => HttpMethod.Delete,
                        _ => HttpMethod.Get
                        //   _ => throw new InvalidOperationException($"Unknown attribute {attribute?.AttributeClass?.Name}")
                    };

                    var routeAttr = FindAttribute(methodSymbol, a => a.OriginalDefinition.ToString() == "Microsoft.AspNetCore.Mvc.RouteAttribute");

                    if (routeAttr != null)
                    {
                        route = routeAttr.ConstructorArguments.FirstOrDefault().Value.ToString() ?? string.Empty;
                    }

                    var customFormatterAttribute = FindAttribute(methodSymbol, a => a.Name == "FormFormatterAttribute");

                    bool useCustomFormatter = customFormatterAttribute != null;


                    var parameters = methodSymbol.Parameters.Where(t => true)
                        .Select(delegate (IParameterSymbol t) { return new ParameterMapping(t.Name, new Parameter(t.Type.ToString(), t.HasExplicitDefaultValue, t.HasExplicitDefaultValue ? t.ExplicitDefaultValue : null)); })
                        .ToArray();
                    var bodyParameter = methodSymbol.Parameters.Where(t => (!IsPrimitive(t.Type)) || t.GetAttributes().Any(e => e.AttributeClass?.Name == "FromBodyAttribute"))
                        .Select(t => new ParameterMapping(t.Name, new Parameter(t.Type.ToString(), t.HasExplicitDefaultValue, t.HasExplicitDefaultValue ? t.ExplicitDefaultValue : null)))
                        .FirstOrDefault();

                    yield return new ActionRoute(name, method, route, returnType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), useCustomFormatter, parameters, bodyParameter);
                }
            }
        }

        private static bool InheritsFrom(INamedTypeSymbol classDeclaration, INamedTypeSymbol targetBaseType)
        {
            var currentDeclared = classDeclaration;
            //StringBuilder sb = new StringBuilder();
            //sb.Append(classDeclaration.Name);
            //foreach (var item in classDeclaration.Interfaces)
            //{
            //    sb.Append($" : {item.Name}");
            //    if (item.Interfaces.Any())
            //    {
            //        foreach (var item2 in item.Interfaces)
            //        {
            //            sb.Append($" : {item2.Name}");
            //        }
            //        sb.AppendLine();
            //    }
            //}

            var bi = classDeclaration.Interfaces.FirstOrDefault(i => i.Equals(targetBaseType, SymbolEqualityComparer.Default));

            //File.WriteAllText($"C:\\Temp\\DerivedInterface{currentDeclared.Name}.txt", sb.ToString());
            File.WriteAllText($"C:\\Temp\\matched-interface-{currentDeclared.Name}.txt", bi?.Name ?? "NULL");

            //var res = classDeclaration.AllInterfaces.Any(i => i.Equals(targetBaseType, SymbolEqualityComparer.Default));
            //return res;
            if(bi != null)
                return true;

            foreach (var item in classDeclaration.Interfaces)
            {
                var res = InheritsFrom(item, targetBaseType);
                if (res)
                {
                    return true;
                }
            }

            return false;
            //while (currentDeclared.Interfaces != null && currentDeclared.Interfaces.Any())
            //{

            //}


            //while (currentDeclared.BaseType != null)
            //{
            //    var currentBaseType = currentDeclared.BaseType;

            //    if (currentBaseType.Equals(targetBaseType, SymbolEqualityComparer.Default))
            //    {
            //        return true;
            //    }

            //    currentDeclared = currentDeclared.BaseType;
            //}

            //return false;
        }

        private static bool IsPrimitive(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Byte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Char:
                case SpecialType.System_String:
                    return true;
            }

            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Enum:
                    return true;
            }

            return false;
        }


        private static AttributeData? FindAttribute(ISymbol symbol, Func<INamedTypeSymbol, bool> selectAttribute)
            => symbol
                .GetAttributes()
                .LastOrDefault(a => a?.AttributeClass != null && selectAttribute(a.AttributeClass));

        private static string GetFullQualifiedName(ISymbol symbol)
        {
            var containingNamespace = symbol.ContainingNamespace;
            if (!containingNamespace.IsGlobalNamespace)
                return containingNamespace.ToDisplayString() + "." + symbol.Name;

            return symbol.Name;
        }
    }
}