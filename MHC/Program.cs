using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manhood;

namespace MHC
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


            Console.Title = "Manhood";
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var mh = new Engine(Directory.Exists("dictionary") ? "dictionary" : null, NsfwFilter.Allow);
            
            while (true)
            {
                Console.Write("manhood> ");
                var input = Console.ReadLine();
#if DEBUG
                Console.WriteLine(mh.Do(input));
#else
                try
                {
                    Console.WriteLine(mh.Do(input));
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
    }
}
