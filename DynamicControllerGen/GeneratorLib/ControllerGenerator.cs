using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace GeneratorLib
{
    [Generator]
    public class ControllerGenerator : ISourceGenerator
    {
        private const string FilePath = "ControllerText.txt";
        private static readonly DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "MYXMLGEN001",
                                                                                              title: "Couldn't find file",
                                                                                              messageFormat: "Couldn't find file '{0}'",
                                                                                              category: "MyGenerator",
                                                                                              DiagnosticSeverity.Warning,
                                                                                              isEnabledByDefault: true);

        public void Execute(GeneratorExecutionContext context)
        {
            // find anything that matches our files
            //var file = Path.Combine(Path.GetDirectoryName(typeof(ControllerGenerator).Assembly.Location), FilePath);
            
            var file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FilePath);
            context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None, file));

            var content = ResourceHelper.GetResourceFileContentAsString(FilePath);

            //var content = File.ReadAllText(file); //$".\\{FilePath}"
            {
                // do some transforms based on the file context
                string output = content.ToString();

                var sourceText = SourceText.From(output, Encoding.UTF8);

                //context.AddSource($"{Path.GetFileNameWithoutExtension(FilePath)}.g.cs", sourceText);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
        }
    }
}
