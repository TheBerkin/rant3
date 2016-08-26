using System;
using System.IO;

using Rant.Vocabulary;

namespace Rant.Tools.DicSort
{
	public static class TableSorter
	{
		public static void GetHelp()
		{
			Console.WriteLine("Usage:");

			Console.WriteLine("  rant sort [paths...] [--diff]");
			Console.WriteLine("    - Sorts tables in the current directory.");
			Console.WriteLine("      --diff: Specifies that entries should be diffmarked.");
		}

		public static void Run()
		{
			var paths = CmdLine.GetPaths();

			if (paths.Length == 0)
			{
				foreach (string path in Directory.GetFiles(Environment.CurrentDirectory, "*.dic", SearchOption.AllDirectories))
				{
					Console.WriteLine($"Processing {path}...");
					ProcessDicFile(path);
				}
			}
			else
			{
				foreach (string path in paths)
				{
					if (path.EndsWith(".dic"))
					{
						Console.WriteLine($"Processing {path}...");
						ProcessDicFile(path);
					}
					else if (!Path.HasExtension(path))
					{
						foreach (string file in Directory.GetFiles(path, "*.dic", SearchOption.AllDirectories))
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
			var table = RantDictionaryTable.FromFile(path);
			table.Save(path, CmdLine.Flag("diff"));
		}
	}
}