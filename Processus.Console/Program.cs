using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Processus.Console
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


            System.Console.Title = "Processus Console";
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var mh = new Engine(Directory.Exists("dictionary") ? "dictionary" : null, NsfwFilter.Allow);
            
            while (true)
            {
                System.Console.Write("processus> ");
                var input = System.Console.ReadLine();
#if DEBUG
                foreach (var chan in mh.Do(input))
                {
                    Console.ForegroundColor = chan.Name == "main" ? ConsoleColor.Cyan : ConsoleColor.Green;
                    Console.WriteLine("{0} ({1}):", chan.Name, chan.Visiblity);
                    Console.ResetColor();
                    Console.WriteLine(chan.Value);
                }
#else
                try
                {
                    foreach (var chan in mh.Do(input))
                    {
                        System.Console.ForegroundColor = chan.Name == "main" ? ConsoleColor.Cyan : ConsoleColor.Green;
                        System.Console.WriteLine("{0} ({1}):", chan.Name, chan.Visiblity);
                        System.Console.ResetColor();
                        System.Console.WriteLine(chan.Value);
                    }
                }
                catch (Exception e)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(e.Message);
                    System.Console.ResetColor();
                }
#endif
            }
        }
    }
}
