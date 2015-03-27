using System;
using System.IO;
using System.Globalization;
using Rant;
using Rant.Vocabulary;

using System.Console;

using RantConsole.CmdLine;

namespace RantConsole
{
    class Program
    {
        public const double PATTERN_TIMEOUT = 10.0;

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;            

            var file = Property("file");
            var dicPath = Property("dict");
            var pkgPath = Property("package");

            long seed = 0;
            bool useCustomSeed = Int64.TryParse(Property("seed"), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out seed);

            Title = "Rant Console" + (Flag("nsfw") ? " [NSFW]" : "");

            RantEngine.DefaultNsfwFilter = Flag("nsfw") ? NsfwFilter.Allow : NsfwFilter.Disallow;
            var rant = new RantEngine();

            try
            {
                if (!String.IsNullOrEmpty(dicPath)) rant.Dictionary = RantDictionary.FromDirectory(dicPath, RantEngine.DefaultNsfwFilter);
                if (!String.IsNullOrEmpty(pkgPath)) rant.LoadPackage(pkgPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load content: \{ex}");
            }

            rant.AddHook("load", hArgs => hArgs.Length != 1 ? "" : rant.DoFile(hArgs[0]));

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
                PrintOutput(rant.Do(input, 0, PATTERN_TIMEOUT));
#else
                try
                {
                    PrintOutput(rant.Do(input, 0, PATTERN_TIMEOUT));
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

        static void PrintOutput(RantOutput output, bool file = false)
        {
            bool writeToFile = !String.IsNullOrEmpty(Property("out"));
            foreach (var chan in output)
            {
                if (chan.Name != "main")
                {
                    if (Flag("main")) continue;
                    if (!writeToFile)
                    {
                        ForegroundColor = ConsoleColor.Green;
                        WriteLine("\{chan.Name} (\{chan.Visiblity}):");
                        ResetColor();
                    }
                }
                ForegroundColor = ConsoleColor.White;
                if (chan.Length > 0)
                {   
                    if (file && writeToFile)
                    {
                        var path = Property("out");
                        File.WriteAllText(Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)
                            + (chan.Name != "main" ? ".\{chan.Name}" : "" + "." + Path.GetExtension(path))), chan.Value); 
                    }
                    else
                    {
                        WriteLine(chan.Value);
                    }
                }
                else if (!writeToFile)
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
