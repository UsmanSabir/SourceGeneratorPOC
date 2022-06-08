using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneratorLib
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // begin creating the source we'll inject into the users compilation
            StringBuilder sourceBuilder = new StringBuilder(@"
using System;
namespace HelloWorldG
{
    public static class HelloWorld
    {
        public static void SayHello() 
        {
Console.WriteLine(""Hello Usman Sabir generated code!"");
            Console.WriteLine(""Hello from generated code!"");
            Console.WriteLine(""The following syntax trees existed in the compilation that created this program:"");
");

            var compilation = context.Compilation;
            //Debugger.Launch();
            var controllerRoutes = compilation.SyntaxTrees
                .Select(t => compilation.GetSemanticModel(t))
                .Select(Scanner.ScanForControllers)
                .SelectMany(c => c)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var controllerRoute in controllerRoutes)
            {
                Console.WriteLine(controllerRoute.Name);
                sb.AppendLine(controllerRoute.Name + " : " + String.Join(",", controllerRoute.Actions.Select(s=>s.Name))
                    + " <> " + String.Join(",", controllerRoute.Actions.Select(a=>a.ReturnTypeName))
                    + " <|> " + String.Join(",", controllerRoute.Actions.SelectMany(a => a.Mapping).Select(m=>m.Key + ":"+ m.Parameter.FullTypeName))
                    + " <||> " + String.Join(",", controllerRoute.Actions.Select(a => a.Body?.Key + ":" + a.Body?.Parameter.FullTypeName))
                    );
            }

            File.WriteAllText("C:\\Temp\\code.txt", sb.ToString());

            // using the context, get a list of syntax trees in the users compilation
            IEnumerable<SyntaxTree> syntaxTrees = context.Compilation.SyntaxTrees;
            // add the filepath of each tree to the class we're building
            foreach (SyntaxTree tree in syntaxTrees)
            {
                sourceBuilder.AppendLine($@"Console.WriteLine(@"" - {tree.FilePath}"");");
                

            }

            // finish creating the source to inject
            sourceBuilder.Append(@"
        }
    }
}");

            // inject the created source into the users compilation
            context.AddSource("helloWorldGenerated.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
        }

        
    }
}
