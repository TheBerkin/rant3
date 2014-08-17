using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Manhood;

namespace MHC
{
    class Program
    {
        static readonly Dictionary<string, string> Arguments = new Dictionary<string, string>();
        private static readonly HashSet<string> Flags = new HashSet<string>();

        private static string GetArg(string name)
        {
            string arg;
            if (!Arguments.TryGetValue(name.ToLower(), out arg))
            {
                arg = "";
            }
            return arg;
        }

        static void Main(string[] args)
        {
            foreach (var argKeyVal in args.Where(arg => arg.StartsWith("-")).Select(arg => arg.TrimStart('-').Split(new[] {'='}, 2)))
            {
                if (argKeyVal.Length == 2)
                {
                    Arguments[argKeyVal[0].ToLower().Trim()] = argKeyVal[1];
                }
                else
                {
                    Flags.Add(argKeyVal[0]);
                }
            }

            Console.Title = "Manhood";
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string file = GetArg("file");
            string dicpath = GetArg("dictionary");
            bool nsfw = Flags.Contains("nsfw");

            try
            {
                var mh = new ManhoodContext(dicpath, nsfw ? NsfwFilter.Allow : NsfwFilter.Disallow);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Generating...\n");
                Console.ResetColor();

                var watch = new Stopwatch();
                watch.Start();
                var output = mh.DoFile(file);
                watch.Stop();
                foreach (var channel in output)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(channel.Name);
                    Console.Write(" : ");
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
