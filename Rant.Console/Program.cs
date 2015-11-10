using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Linq;

using Rant;
using Rant.Vocabulary;

using static System.Console;

using static Rant.Common.CmdLine;

namespace RantConsole
{
    class Program
    {
        public const double PATTERN_TIMEOUT = 10.0;
        public static readonly string FILE = Property("file");
        public static readonly string DIC_PATH = Property("dict");
        public static readonly string PKG_PATH = Property("package");
        public static readonly long SEED;
        public static readonly bool USE_SEED;

        static Program()
        {
            USE_SEED = Int64.TryParse(Property("seed"), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out SEED);
        }

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Title = "Rant Console" + (Flag("nsfw") ? " [NSFW]" : "");

            var rant = new RantEngine();

#if !DEBUG
            try
            {
#endif
                if (!String.IsNullOrEmpty(DIC_PATH)) rant.Dictionary = RantDictionary.FromDirectory(DIC_PATH);

                if (!String.IsNullOrEmpty(PKG_PATH))
                {
#if DEBUG
                    Stopwatch timer = Stopwatch.StartNew();
#endif
                    rant.LoadPackage(PKG_PATH);
#if DEBUG
                    timer.Stop();
                    WriteLine($"Package loading: {timer.ElapsedMilliseconds}ms");
#endif
                }
#if !DEBUG
            }
            catch (Exception e)
            {
                ForegroundColor = ConsoleColor.Cyan;
                WriteLine($"Dictionary load error: {e.Message}");
            }
#endif
            if (Flag("nsfw")) rant.Dictionary.IncludeHiddenClass("nsfw");

            if (!String.IsNullOrEmpty(FILE))
            {
#if !DEBUG
                try
                {
#endif
                    PrintOutput(rant, File.ReadAllText(FILE));
#if !DEBUG
                }
                catch (Exception ex)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine(ex.Message);
                    ResetColor();
                }
#endif

                if (Flag("wait")) ReadKey(true);
                return;
            }

            while (true)
            {
                ForegroundColor = Flag("nsfw") ? ConsoleColor.DarkRed : ConsoleColor.Gray;
                Write("RANT> "); // real number symbol
                ForegroundColor = ConsoleColor.White;
                PrintOutput(rant, ReadLine());
            }
        }

        static void PrintOutput(RantEngine engine, string source, bool isFile = false)
        {
            try
            {
                var sw = new Stopwatch();

                sw.Start();
                var pattern = isFile
                    ? RantPattern.FromFile(source)
                    : RantPattern.FromString(source);
                sw.Stop();
                var compileTime = sw.Elapsed;

                sw.Restart();
                var output = USE_SEED
                    ? engine.Do(pattern, SEED, 0, PATTERN_TIMEOUT)
                    : engine.Do(pattern, 0, PATTERN_TIMEOUT);
                sw.Stop();

                var runTime = sw.Elapsed;

                bool writeToFile = !String.IsNullOrEmpty(Property("out"));
                foreach (var chan in output)
                {
                    if (chan.Name != "main")
                    {
                        if (Flag("main")) continue;
                        if (!writeToFile)
                        {
                            ForegroundColor = ConsoleColor.DarkCyan;
                            WriteLine($"{chan.Name}:");
                            ResetColor();
                        }
                    }
                    ForegroundColor = ConsoleColor.Green;
                    if (chan.Value.Length > 0)
                    {
                        if (pattern.Type == RantPatternOrigin.File && writeToFile)
                        {
                            var path = Property("out");
                            File.WriteAllText(
                                Path.Combine(Path.GetDirectoryName(path),
                                Path.GetFileNameWithoutExtension(path) +
                                (chan.Name != "main"
                                    ? $".{chan.Name}"
                                    : "" + "." + Path.GetExtension(path))),
                                chan.Value);
                        }
                        else
                        {
#if DEBUG
                            WriteLine($"'{chan.Value}'");
#else
                            WriteLine(chan.Value);
#endif
                        }
                    }
                    else if (!writeToFile)
                    {
                        ForegroundColor = ConsoleColor.DarkGray;
                        if (pattern.Type != RantPatternOrigin.File) WriteLine("[Empty]");
                    }
                    ResetColor();
                    WriteLine();
                }

                if ((pattern.Type != RantPatternOrigin.File || Flag("wait")) && !Flag("nostats"))
                {
                    PrintStats(
                        new Stat("Seed",
                            $"{output.Seed:X16}{(output.BaseGeneration != 0 ? ":" + output.BaseGeneration : String.Empty)}"),
                        new Stat("Compile Time", compileTime.ToString("c")),
                        new Stat("Run Time", runTime.ToString("c"))
                        );
                    WriteLine();
                }
            }
#if !DEBUG
            catch (RantRuntimeException e)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"Runtime error: {e.Message}");
            }
            catch (RantCompilerException e)
            {
                ForegroundColor = ConsoleColor.Yellow;
                WriteLine($"Compiler error: {e.Message}");
            }
            catch (Exception e)
            {
                WriteLine(e.ToString()); // Print the whole stack trace if it isn't a Rant error
            }
#endif
            finally
            {
                ResetColor();
            }
        }

        static void PrintStats(params Stat[] stats)
        {
            int alignment = stats.Max(s => s.Name.Length);
            var fmtString = $"{{0, {alignment}}}: ";
            foreach (var stat in stats)
            {
                ForegroundColor = ConsoleColor.DarkGray;
                Write(fmtString, stat.Name);
                ForegroundColor = ConsoleColor.DarkMagenta;
                WriteLine(stat.Value);
            }
            ResetColor();
        }

        private class Stat
        {
            public readonly string Name;
            public readonly object Value;

            public Stat(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
