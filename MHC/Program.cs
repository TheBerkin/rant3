using System;
using System.Diagnostics;
using System.IO;
using Manhood;

namespace MHC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Manhood";
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (args.Length < 1)
            {
                Console.WriteLine("No file specified!");
                Console.ReadKey();
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File not found!");
                Console.ReadKey();
                return;
            }

            var mh = Directory.Exists("dictionary")
                ? new ManhoodContext("dictionary")
                : new ManhoodContext();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Generating...");
            Console.ResetColor();

            try
            {
                var watch = new Stopwatch();
                watch.Start();
                var output = mh.Do(File.ReadAllText(args[0]));
                watch.Stop();
                foreach (var channel in output)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(channel.Name);
                    Console.Write(" = ");
                    Console.ResetColor();
                    Console.WriteLine(channel.Output);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nGenerated in {0}", watch.Elapsed);
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Holy shit! An error!");
                Console.WriteLine(e.ToString());
            }
            
            Console.ReadKey();
        }
    }
}
