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
            Console.WriteLine("  general rave pack options:");
            Console.WriteLine("    -compression [true|false]: Enable or disable LZMA compression. Defaults to true.");
            Console.WriteLine("    -string-table [mode]: Set the string table mode. 0 = none, 1 = keys, 2 = keys and values.");
            Console.WriteLine("                          Defaults to none.");
		}

		public static void Run()
		{
			var pkg = new RantPackage();
			var paths = GetPaths();
			var outputPath = Property("out", Path.Combine(
				Directory.GetParent(Environment.CurrentDirectory).FullName,
				Path.GetFileName(Environment.CurrentDirectory) + ".rantpkg"));
            var compress = Property("compression", "true") == "true";
            var stringTableMode = int.Parse(Property("string-table", "0"));
            if(stringTableMode < 0 || stringTableMode > 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid string table mode.");
                Console.ResetColor();
                return;
            }
            var modeEnum = (Rant.IO.Bson.BsonStringTableMode)stringTableMode;
			
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

            Console.WriteLine("String table mode: " + modeEnum.ToString().Replace("A", " A").Replace("V", " V").ToLower());
            Console.WriteLine("Compression: " + (compress ? "yes" : "no"));

            Console.WriteLine(compress ? "Compressing and saving..." : "Saving...");
			pkg.Save(outputPath, compress, modeEnum);

			Console.WriteLine("\nPackage saved to " + outputPath);

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
				var table = RantDictionaryTable.FromFile(path);
				package.AddTable(table);
			}
		}
	}
}