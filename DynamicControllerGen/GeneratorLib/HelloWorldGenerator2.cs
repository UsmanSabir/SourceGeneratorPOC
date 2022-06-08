using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace GeneratorLib
{
    [Generator]
    public class HelloWorldGenerator2 : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // find anything that matches our files
            var myFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".txt"));
            foreach (var file in myFiles)
            {
                var content = file.GetText(context.CancellationToken);

                // do some transforms based on the file context
                string output =content.ToString();

                var sourceText = SourceText.From(output, Encoding.UTF8);

                context.AddSource($"{Path.GetFileName(file.Path)}generated.cs", sourceText);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
        }
    }
}
