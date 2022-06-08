// See https://aka.ms/new-console-template for more information
using TestConsoleApp;

Console.WriteLine("Hello, World!");

var t = typeof(GeneratorLib.HelloWorldGenerator5).Assembly;
string[] names = t.GetManifestResourceNames();
foreach (string name in names) Console.WriteLine(name);

string source = @"
namespace Foo
{
    class C
    {
        void M()
        {
        }
    }
}";

var (diagnostics, output) = Helper.GetGeneratedOutput(source);

if (diagnostics.Length > 0)
{
    Console.WriteLine("Diagnostics:");
    foreach (var diag in diagnostics)
    {
        Console.WriteLine("   " + diag.ToString());
    }
    Console.WriteLine();
    Console.WriteLine("Output:");
}

Console.WriteLine(output);
