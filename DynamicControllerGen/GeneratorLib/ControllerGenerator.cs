using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
using Scriban;
using Microsoft.CodeAnalysis.CSharp;

namespace GeneratorLib
{
    [Generator]
    public class ControllerGenerator : ISourceGenerator
    {
        private const string FilePath = "Controller.tpl";
        private static readonly DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "MYXMLGEN001",
                                                                                              title: "Couldn't find file",
                                                                                              messageFormat: "Couldn't find file '{0}'",
                                                                                              category: "MyGenerator",
                                                                                              DiagnosticSeverity.Warning,
                                                                                              isEnabledByDefault: true);

        public void Execute(GeneratorExecutionContext context)
        {
            System.Diagnostics.Debugger.Launch();

            
            var compilation = context.Compilation;
            var controllerRoutes = compilation.SyntaxTrees
                .Select(t => compilation.GetSemanticModel(t))
                .Select(Scanner.ScanForControllers)
                .SelectMany(c => c)
                .ToArray();

            var content = ResourceHelper.GetResourceFileContentAsString(FilePath);
            var template = Template.Parse(content);
            if (template.HasErrors)
            {
                var errors = string.Join(" | ", template.Messages.Select(x => x.Message));
                throw new InvalidOperationException($"Template parse error: {errors}");
            }

            var nameSpace = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
            if (!nameSpace)
            {
                rootNamespace = compilation.Assembly?.ContainingNamespace?.ToDisplayString();
                if (string.IsNullOrWhiteSpace(rootNamespace))
                {
                    rootNamespace = compilation.AssemblyName;
                }
            }
            if (string.IsNullOrWhiteSpace(rootNamespace))
                rootNamespace = "Generated";

            foreach (var controllerRoute in controllerRoutes)
            {
                
                var controllerName = $"{controllerRoute.Name}Controller";
                var className = $"{controllerRoute.ClassName}";
                var classFullName = controllerRoute.ClassFullName;

                ControllerModel model = new ControllerModel(rootNamespace, controllerName, className, classFullName, controllerRoute.Actions);
                var result = template.Render(model, memberRenamer: member => member.Name);
                result = SyntaxFactory.ParseCompilationUnit(result)
                      .NormalizeWhitespace()
                      .GetText()
                      .ToString();

                var sourceText = SourceText.From(result, Encoding.UTF8);
                context.AddSource($"{controllerName}.g.cs", sourceText);
            }
            //var content = File.ReadAllText(file); //$".\\{FilePath}"
            {
                // do some transforms based on the file context
                
                

                //context.AddSource($"{Path.GetFileNameWithoutExtension(FilePath)}.g.cs", sourceText);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
        }
    }
}
