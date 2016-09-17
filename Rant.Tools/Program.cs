using System;
using System.IO;

using Rant.Tools.Packer;

using static System.Console;

namespace Rant.Tools
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (CmdLine.Flag("version"))
			{
				WriteLine($"Rant {typeof(RantEngine).Assembly.GetName().Version}");
			}

			if (string.IsNullOrEmpty(CmdLine.Command))
			{
				Help.Print();
				return;
			}
#if !DEBUG
			try
			{
#endif
				switch (CmdLine.Command)
				{
					case "pack":
					{
						PackGenerator.Run();
						break;
					}
					case "build":
					{
						var paths = CmdLine.GetPaths();
						foreach (var path in paths.Length == 0 ? Directory.GetFiles(Environment.CurrentDirectory, "*.rant") : paths)
						{
							Build(path);
						}
						Console.WriteLine("Done");
						break;
					}
					default:
						WriteLine($"Unknown command: '{CmdLine.Command}'");
						break;
				}
#if !DEBUG
			}
			catch (Exception ex)
			{
				ForegroundColor = ConsoleColor.Red;
				WriteLine(ex.Message);
				ResetColor();
				Environment.Exit(1);
			}
#endif
		}

		private static void Build(string path)
		{
			try
			{
				Console.WriteLine($"Building: {path}");

				var pgm = RantProgram.CompileFile(path);

				pgm.SaveToFile(Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".rantpgm"));
			}
			catch (RantCompilerException ex)
			{
				Console.WriteLine($"Build failed for {path}.\n{ex.Message}\n");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while building: {ex.Message}");
			}
		}
	}
}