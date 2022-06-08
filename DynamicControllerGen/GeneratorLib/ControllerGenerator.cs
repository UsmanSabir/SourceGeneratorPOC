using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
using Scriban;

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

            //if (!context.Compilation.ReferencedAssemblyNames.Any(ai => ai.Name.Equals("Scriban", StringComparison.OrdinalIgnoreCase)))
            //{
            //    //context.ReportDiagnostic(/*error or warning*/);
            //}

            var compilation = context.Compilation;
            var controllerRoutes = compilation.SyntaxTrees
                .Select(t => compilation.GetSemanticModel(t))
                .Select(Scanner.ScanForControllers)
                .SelectMany(c => c)
                .ToArray();

            var content = ResourceHelper.GetResourceFileContentAsString(FilePath);
            var tpl = Template.Parse(content);
            
            foreach (var controllerRoute in controllerRoutes)
            {
                var controllerName = $"{controllerRoute.Name}Controller";
                ControllerModel model = new ControllerModel(compilation.GlobalNamespace.ToDisplayString(), controllerName, controllerRoute.Actions);
                var res = tpl.Render(model);
                var sourceText = SourceText.From(res, Encoding.UTF8);
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
