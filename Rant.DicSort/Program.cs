using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Vocabulary;

namespace Rant.DicSort
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    foreach (var path in Directory.GetFiles(Environment.CurrentDirectory, "*.dic", SearchOption.AllDirectories))
                    {
                        Console.WriteLine("Processing \{path}...");
                        ProcessDicFile(path);
                    }
                }
                else
                {
                    foreach (var path in args)
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
            table.Save(path);
        }
    }
}
