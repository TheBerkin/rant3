using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
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

            RantEngine.DefaultNsfwFilter = Flag("nsfw") ? NsfwFilter.Allow : NsfwFilter.Disallow;
            var rant = new RantEngine();

			try
            {
                if (!String.IsNullOrEmpty(DIC_PATH)) rant.Dictionary = RantDictionary.FromDirectory(DIC_PATH, RantEngine.DefaultNsfwFilter);
                if (!String.IsNullOrEmpty(PKG_PATH)) rant.LoadPackage(PKG_PATH);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load content: {ex}");
            }

            rant.AddHook("load", hArgs => hArgs.Length != 1 ? "" : rant.DoFile(hArgs[0]));
			
			if (!String.IsNullOrEmpty(FILE))
            {
                try
                {
                    PrintOutput(rant, FILE);                
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
	            PrintOutput(rant, ReadLine());
            }
        }

        static void PrintOutput(RantEngine engine, string source, bool isFile = false)
        {
	        try
	        {
		        var pattern = isFile
			        ? RantPattern.FromFile(source)
			        : RantPattern.FromString(source);

				var sw = new Stopwatch();
		        sw.Start();
		        var output = USE_SEED
					? engine.Do(pattern, SEED, 0, PATTERN_TIMEOUT)
					: engine.Do(pattern, 0, PATTERN_TIMEOUT);
		        sw.Stop();

				bool writeToFile = !String.IsNullOrEmpty(Property("out"));
				foreach (var chan in output)
				{
					if (chan.Name != "main")
					{
						if (Flag("main")) continue;
						if (!writeToFile)
						{
							ForegroundColor = ConsoleColor.DarkGray;
							WriteLine($"{chan.Name} ({chan.Visiblity}):");
							ResetColor();
						}
					}
					ForegroundColor = ConsoleColor.Green;
					if (chan.Length > 0)
					{
						if (pattern.Type == RantPatternSource.File && writeToFile)
						{
							var path = Property("out");
							File.WriteAllText(Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)
								+ (chan.Name != "main" ? $".{chan.Name}" : "" + "." + Path.GetExtension(path))), chan.Value);
						}
						else
						{
							WriteLine(chan.Value);
						}
					}
					else if (!writeToFile)
					{
						ForegroundColor = ConsoleColor.DarkGray;
						if (pattern.Type != RantPatternSource.File) WriteLine("[Empty]");
					}
					ResetColor();
				}

				if ((pattern.Type != RantPatternSource.File || Flag("wait")) && !Flag("nostats"))
				{
					WriteLine();
					ForegroundColor = ConsoleColor.DarkGray;
					Write("Seed: ");
					ForegroundColor = ConsoleColor.DarkMagenta;
					WriteLine($"{output.Seed:X16}");
					ForegroundColor = ConsoleColor.DarkGray;
					if (output.BaseGeneration != 0)
					{
						Write("Base Gen: ");
						ForegroundColor = ConsoleColor.DarkMagenta;
						WriteLine(output.BaseGeneration);
					}
					ForegroundColor = ConsoleColor.DarkGray;
					Write("Time: ");
					ForegroundColor = ConsoleColor.DarkMagenta;
					WriteLine(sw.Elapsed.ToString("c"));
					ResetColor();
				}
			}
#if !DEBUG
	        catch (Exception e)
	        {
		        if (e is RantRuntimeException)
		        {
			        ForegroundColor = ConsoleColor.Red;
			        WriteLine($"Runtime error: {e.Message}");
		        }
		        else if (e is RantCompilerException)
		        {
			        ForegroundColor = ConsoleColor.Yellow;
			        WriteLine($"Compiler error: {e.Message}");
		        }
		        else
		        {
			        WriteLine(e.ToString()); // Print the whole stack trace if it isn't a Rant error
		        }
	        }
#endif
	        finally
	        {
				ResetColor();
			}
        }
    }
}
