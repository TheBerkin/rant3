using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant;

namespace PCon
{
    class Program
    {
        private static readonly Dictionary<string, string> Arguments = new Dictionary<string, string>();
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

            var file = GetArg("file");

            Console.Title = "Rant Console" + (Flags.Contains("nsfw") ? " [NSFW]" : "");
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var mh = new Engine(Directory.Exists("dictionary") ? "dictionary" : null, Flags.Contains("nsfw") ? NsfwFilter.Allow : NsfwFilter.Disallow);

            if (!String.IsNullOrEmpty(file))
            {
                try
                {
                    PrintOutput(mh.DoFile(file));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }

            while (true)
            {   
                Console.Write("rant");
                if (Flags.Contains("nsfw")) Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("> ");
                Console.ResetColor();

                var input = Console.ReadLine();
#if DEBUG
                PrintOutput(mh.Do(input));
#else
                try
                {
                    PrintOutput(mh.Do(input));
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
#endif
            }
        }

        static void PrintOutput(Output output)
        {
            foreach (var chan in output)
            {
                if (chan.Name != "main")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0} ({1}):", chan.Name, chan.Visiblity);
                    Console.ResetColor();
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(chan.Value);
                Console.ResetColor();
            }
        }
    }
}
