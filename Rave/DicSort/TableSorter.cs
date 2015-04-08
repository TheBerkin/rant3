using System;
using System.IO;

using Rant.Vocabulary;

using static Rave.CmdLine;

namespace Rave.DicSort
{
	public static class TableSorter
	{
		public static void GetHelp()
		{
			Console.WriteLine("USAGE\n");

			Console.WriteLine("  rave sort [paths...] [--diff]");
			Console.WriteLine("    - Sorts tables in the current directory.");
			Console.WriteLine("      --diff: Specifies that entries should be diffmarked.");
		}

		public static void Run()
		{
			var paths = GetPaths();

			if (paths.Length == 0)
			{
				foreach (var path in Directory.GetFiles(Environment.CurrentDirectory, "*.dic", SearchOption.AllDirectories))
				{
					Console.WriteLine($"Processing {path}...");
					ProcessDicFile(path);
				}
			}
			else
			{
				foreach (var path in paths)
				{
					if (path.EndsWith(".dic"))
					{
						Console.WriteLine($"Processing {path}...");
						ProcessDicFile(path);
					}
					else if (!Path.HasExtension(path))
					{
						foreach (var file in Directory.GetFiles(path, "*.dic", SearchOption.AllDirectories))
						{
							Console.WriteLine($"Processing {file}...");
							ProcessDicFile(file);
						}
					}
				}
			}
			Console.WriteLine("Done.");
		}

		private static void ProcessDicFile(string path)
		{
			var table = RantDictionaryTable.FromFile(path, NsfwFilter.Allow);
			table.Save(path, Flag("diff"));
		}
	}
}