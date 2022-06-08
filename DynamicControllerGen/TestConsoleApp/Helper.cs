using GeneratorLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    internal class Helper
    {
        internal static (ImmutableArray<Diagnostic>, string) GetGeneratedOutput(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var references = new List<MetadataReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic)
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            var compilation = CSharpCompilation.Create("foo", new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // TODO: Uncomment these lines if you want to return immediately if the injected program isn't valid _before_ running generators
            //
            // ImmutableArray<Diagnostic> compilationDiagnostics = compilation.GetDiagnostics();
            //
            // if (diagnostics.Any())
            // {
            //     return (diagnostics, "");
            // }

            var generator = new ControllerGenerator();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);

            return (generateDiagnostics, outputCompilation.SyntaxTrees.Last().ToString());
        }
    }
}
