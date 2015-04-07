using System;
using System.IO;

using Rant;
using Rant.Vocabulary;

using static Rave.CmdLine;

namespace Rave.Packer
{
	public static class PackGenerator
	{
		public static void GetHelp()
		{
			Console.WriteLine("USAGE\n");

			Console.WriteLine("  rave pack [-out package-path]");
			Console.WriteLine("    - Creates a package from the current directory.");
			Console.WriteLine("      -out: Specifies the output path for the package.");
			Console.WriteLine("  rave pack [content-paths...] [-out package-path]");
			Console.WriteLine("    - Creates a package from the specified directories.");
			Console.WriteLine("      -out: Specifies the output path for the package.");
		}

		public static void Run()
		{
			var pkg = new RantPackage();
			var paths = GetPaths();
			var outputPath = Property("out", Path.Combine(
				Directory.GetParent(Environment.CurrentDirectory).FullName,
				Path.GetFileName(Environment.CurrentDirectory) + ".rantpkg"));
			try
			{
				Console.WriteLine("Packing...");

				if (paths.Length == 0)
				{
					Pack(pkg, Environment.CurrentDirectory);
				}
				else
				{
					foreach (var path in paths)
					{
						Pack(pkg, path);
					}
				}

				pkg.Save(outputPath);

				Console.WriteLine("\nPackage saved to " + outputPath);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Something went wrong while generating your package:\n" + ex);
			}

			Console.ResetColor();
		}

		private static void Pack(RantPackage package, string contentPath)
		{
			foreach (var path in Directory.GetFiles(contentPath, "*.rant", SearchOption.AllDirectories))
			{
				Console.WriteLine("+ " + path);

				var pattern = RantPattern.FromFile(path);
				package.AddPattern(pattern);
			}

			foreach (var path in Directory.GetFiles(contentPath, "*.dic", SearchOption.AllDirectories))
			{
				Console.WriteLine("+ " + path);
				var table = RantDictionaryTable.FromFile(path, NsfwFilter.Allow);
				package.AddTable(table);
			}
		}
	}
}