//using System;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Text;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;
//using System.Reflection;

//namespace GeneratorLib
//{
//    [Generator]
//    public class HelloWorldGenerator5 : ISourceGenerator
//    {
//        private const string FilePath = "Hello5.txt";
//        private static readonly DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "MYXMLGEN001",
//                                                                                              title: "Couldn't find file",
//                                                                                              messageFormat: "Couldn't find file '{0}'",
//                                                                                              category: "MyGenerator",
//                                                                                              DiagnosticSeverity.Warning,
//                                                                                              isEnabledByDefault: true);

//        public void Execute(GeneratorExecutionContext context)
//        {
//            // find anything that matches our files
//            var file = Path.Combine(Path.GetDirectoryName(typeof(HelloWorldGenerator5).Assembly.Location), FilePath);
//            context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None, file));

//            var content = Program2.GetResourceFileContentAsString("SampleCode.txt"); //Hello5.txt

//            //var content = File.ReadAllText(file); //$".\\{FilePath}"
//            {
//                // do some transforms based on the file context
//                string output = content.ToString();

//                var sourceText = SourceText.From(output, Encoding.UTF8);

//                context.AddSource($"{Path.GetFileName(FilePath)}generated.cs", sourceText);
//            }
//        }

//        public void Initialize(GeneratorInitializationContext context)
//        {
//            // No initialization required
//        }
//    }

//    static class Program2
//    {
//        public static string GetResourceFileContentAsString(string fileName)
//        {
//            var assembly = Assembly.GetExecutingAssembly();
//            var resourceName = "GeneratorLib." + fileName; //

//            string resource = null;
//            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
//            {
//                using (StreamReader reader = new StreamReader(stream))
//                {
//                    resource = reader.ReadToEnd();
//                }
//            }
//            return resource;
//        }
//    }
//}
