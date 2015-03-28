using System;
using System.IO;

using Rant.Common.CmdLine;
using Rant.Vocabulary;

namespace Rant.DicSort
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Help)
            {
                Console.WriteLine("USAGE\n");
                Console.WriteLine("dicsort [paths...] [--diff]");
                Console.WriteLine("  - Sorts tables in the current directory.");
                Console.WriteLine("    --diff: Specifies that entries should be diffmarked.");
                return;
            }

            try
            {
                var paths = GetPaths();

                if (paths.Length == 0)
                {
                    foreach (var path in Directory.GetFiles(Environment.CurrentDirectory, "*.dic", SearchOption.AllDirectories))
                    {
                        Console.WriteLine("Processing \{path}...");
                        ProcessDicFile(path);
                    }
                }
                else
                {
                    foreach (var path in paths)
                    {
                        if (path.EndsWith(".dic"))
                        {
                            Console.WriteLine("Processing \{path}...");
                            ProcessDicFile(path);
                        }
                        else if (!Path.HasExtension(path))
                        {
                            foreach (var file in Directory.GetFiles(path, "*.dic", SearchOption.AllDirectories))
                            {
                                Console.WriteLine("Processing \{file}...");
                                ProcessDicFile(file);
                            }
                        }
                    }
                }
                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something awful happened while processing your files:\n\n" + ex.ToString());
                Console.ResetColor();
            }
        }

        static void ProcessDicFile(string path)
        {
            var table = RantDictionaryTable.FromFile(path, NsfwFilter.Allow);
            table.Save(path, Flag("diff"));
        }
    }
}
