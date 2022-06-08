using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace GeneratorLib
{
    [Generator]
    public class HelloWorldGenerator3 : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // begin creating the source we'll inject into the users compilation
            StringBuilder sourceBuilder = new StringBuilder(@"
using System;
namespace HelloWorldG
{
    public static class HelloWorld3
    {
        public static void SayHello() 
        {
Console.WriteLine(""Hello Usman Sabir generated code3333!"");
           
");
                       

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
