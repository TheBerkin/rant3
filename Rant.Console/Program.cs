using System;
using System.Globalization;
using Rant;
using Rant.Vocabulary;

using System.Console;

using RantConsole.CmdLine;

namespace RantConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;            

            var file = Property("file");
            var dicPath = Property("dicpath");

            long seed = 0;
            bool useCustomSeed = Int64.TryParse(Property("seed"), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out seed);

            Title = "Rant Console" + (Flag("nsfw") ? " [NSFW]" : "");            

            var rant = new RantEngine(String.IsNullOrEmpty(dicPath) ? "dictionary" : dicPath, Flag("nsfw") ? NsfwFilter.Allow : NsfwFilter.Disallow);
            rant.Hooks.AddHook("load", hArgs => hArgs.Length != 1 ? "" : rant.DoFile(hArgs[0]));

            if (!String.IsNullOrEmpty(file))
            {
                try
                {
                    PrintOutput(useCustomSeed ? rant.DoFile(file, seed) : rant.DoFile(file), true);                    
                }
                catch (Exception ex)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine(ex.Message);
                    ResetColor();
                }

                if (Flag("wait")) ReadKey(true);
                return;
            }

            while (true)
            {
                ForegroundColor = Flag("nsfw") ? ConsoleColor.Magenta : ConsoleColor.Yellow;
                Write("\u211d> "); // real number symbol
                ResetColor();

                var input = ReadLine();
#if DEBUG
                PrintOutput(rant.Do(input));
#else
                try
                {
                    PrintOutput(rant.Do(input));
                }
                catch (Exception e)
                {
                    ForegroundColor = ConsoleColor.Red;
                    if (e is RantException)
                    {
                        WriteLine(e.Message);
                    }
                    else
                    {
                        WriteLine(e.ToString()); // Print the whole stack trace if it isn't a syntax error
                    }                    
                    ResetColor();
                }
#endif
            }
        }

        static void PrintOutput(Output output, bool file = false)
        {
            foreach (var chan in output)
            {
                if (chan.Name != "main")
                {
                    if (Flag("main")) continue;
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("\{chan.Name} (\{chan.Visiblity}):");
                    ResetColor();
                }
                ForegroundColor = ConsoleColor.White;
                if (chan.Length > 0)
                {
                    WriteLine(chan.Value);
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkGray;
                    if (!file) WriteLine("[Empty]");
                }
                ResetColor();
            }

            if ((!file || Flag("wait")) && !Flag("nostats"))
            {
                WriteLine();
                ForegroundColor = ConsoleColor.DarkGray;
                Write("Seed: ");
                ForegroundColor = ConsoleColor.DarkMagenta;
                WriteLine(String.Format("{0:X16}", output.Seed));
                ForegroundColor = ConsoleColor.DarkGray;
                if (output.BaseGeneration != 0)
                {
                    Write("Base Gen: ");
                    ForegroundColor = ConsoleColor.DarkMagenta;
                    WriteLine(output.BaseGeneration);
                }
                ResetColor();
            }
        }
    }
}
