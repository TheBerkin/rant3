using System;
using System.IO;

using Rant.Vocabulary;

namespace Rant.Package
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "?")
            {
                ShowHelp();
                return;
            }

            var pkg = new RantPackage();

            if (args.Length == 0)
            {
                Console.WriteLine("Loading content...\n");
                try
                {
                    foreach (var path in Directory.GetFiles(Environment.CurrentDirectory, "*.rant", SearchOption.AllDirectories))
                    {
                        Console.WriteLine("+ " + path);
                        
                        var pattern = RantPattern.FromFile(path);
                        pkg.AddPattern(pattern);
                    }

                    foreach (var path in Directory.GetFiles(Environment.CurrentDirectory, "*.dic", SearchOption.AllDirectories))
                    {
                        Console.WriteLine("+ " + path);
                        var table = RantDictionaryTable.FromFile(path, NsfwFilter.Allow);
                        pkg.AddTable(table);
                    }

                    var outputPath = Path.Combine(
                        Directory.GetParent(Environment.CurrentDirectory).FullName,
                        Path.GetFileName(Environment.CurrentDirectory) + ".rantpkg");

                    Console.WriteLine("Packing...");

                    pkg.Save(outputPath);

                    Console.WriteLine("\nPackage saved to " + outputPath);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Something went wrong while generating your package:\n" + ex);
                }
            }
            else
            {
                ShowHelp();
            }

            Console.ResetColor();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Rant Package Utility\n");
            Console.WriteLine("Usage:\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("    rantpkg");
            Console.ResetColor();
            Console.WriteLine("    Creates a package from the current directory.\n");
        }
    }
}
