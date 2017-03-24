using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Rant;
using Rant.Vocabulary;

using static System.Console;

using static Rant.Common.CmdLine;
using Rant.Formats;

namespace RantConsole
{
    internal class Program
    {
#if !DEBUG
        public const double PATTERN_TIMEOUT = 10.0;
#else
		public const double PATTERN_TIMEOUT = 0.0;
#endif
        public static readonly string FILE = GetPaths().FirstOrDefault();
        public static readonly string PKG_DIR = Property("pkgdir");
        public static readonly string LEGACY_DIC_PATH = Property("ldict");
        public static readonly long SEED;
        public static readonly bool USE_SEED;

        static Program()
        {
            USE_SEED = long.TryParse(Property("seed"), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out SEED);
        }

        private static void Main(string[] args)
        {
            OutputEncoding = Encoding.Unicode;
            Title = "Rant Console" + (Flag("nsfw") ? " [NSFW]" : "");

            var rant = new RantEngine();

#if !DEBUG
            try
            {
#endif
                if (!string.IsNullOrEmpty(LEGACY_DIC_PATH))
                {
                    var tables =
                        Directory
                            .GetFiles(LEGACY_DIC_PATH)
                            .Where(f => Path.GetExtension(f) == ".dic")
                            .Select(f => RantDictionaryTable.FromLegacyFile(f));
                    rant.Dictionary = new RantDictionary(tables);
                }

                if (!string.IsNullOrEmpty(PKG_DIR))
                {
#if DEBUG
                    Stopwatch timer = Stopwatch.StartNew();
#endif
                    foreach (var pkg in Directory.GetFiles(PKG_DIR, "*.rantpkg"))
                    {
                        rant.LoadPackage(pkg);
                    }
#if DEBUG
                    timer.Stop();
                    WriteLine($"Package loading: {timer.ElapsedMilliseconds}ms");
#endif
                }
                else
                {
                    foreach (string pkg in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.rantpkg", SearchOption.AllDirectories))
                    {
                        rant.LoadPackage(pkg);
                    }
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

            if (!string.IsNullOrEmpty(FILE))
            {
#if !DEBUG
                try
                {
#endif
                    PrintOutput(rant, FILE, true);
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
                Write("RANT> ");
                ForegroundColor = ConsoleColor.White;
                string input = ReadLine();
                if (input == null)
                {
                    return;
                }
                PrintOutput(rant, input);
            }
        }

        private static void PrintOutput(RantEngine engine, string source, bool isFile = false)
        {
            try
            {
                var sw = new Stopwatch();

                sw.Start();
                var pattern = isFile
                    ? source.EndsWith(".rantpgm")
                        ? RantProgram.LoadFile(source)
                        : RantProgram.CompileFile(source)
                    : RantProgram.CompileString(source);
                sw.Stop();

                var compileTime = sw.Elapsed;

                sw.Restart();
                var output = USE_SEED
                    ? engine.Do(pattern, SEED, 0, PATTERN_TIMEOUT)
                    : engine.Do(pattern, 0, PATTERN_TIMEOUT);
                sw.Stop();

                var runTime = sw.Elapsed;

                bool writeToFile = !string.IsNullOrEmpty(Property("out"));
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
                        if (pattern.Type == RantProgramOrigin.File && writeToFile)
                        {
                            string path = Property("out");
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
                            WriteLine(chan.Value.Normalize(NormalizationForm.FormC));
#endif
                        }
                    }
                    else if (!writeToFile)
                    {
                        ForegroundColor = ConsoleColor.DarkGray;
                        if (pattern.Type != RantProgramOrigin.File) WriteLine("[Empty]");
                    }
                    ResetColor();
                    WriteLine();
                }

                if (!Flag("nostats"))
                {
                    PrintStats(
                        new Stat("Seed",
                            $"{output.Seed:X16}{(output.BaseGeneration != 0 ? ":" + output.BaseGeneration : string.Empty)}"),
                        new Stat(isFile && source.EndsWith(".rantpgm") ? "Load Time" : "Compile Time", compileTime.ToString("c")),
                        new Stat("Run Time", runTime.ToString("c"))
                        );
                    WriteLine();
                }
                if (isFile && Flag("wait")) Console.ReadKey();
            }
#if !DEBUG
            catch (RantRuntimeException e)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(e);
            }
            catch (RantCompilerException e)
            {
                ForegroundColor = ConsoleColor.Yellow;
                WriteLine(e.Message);
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

        private static void PrintStats(params Stat[] stats)
        {
            int alignment = stats.Max(s => s.Name.Length);
            string fmtString = $"{{0, {alignment}}}: ";
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