using System;
using System.Collections.Generic;

namespace Rant.Tools
{
	public static class Help
	{
		private static readonly Dictionary<string, HelpItem> Items = new Dictionary<string, HelpItem>();

		static Help()
		{
			AddHelpItem("docs", "Generates HTML documentation for Rant dictionaries.");
			AddHelpItem("pack", "Packages patterns and dictionaries into a portable archive format that can be loaded directly into Rant.");
			AddHelpItem("sort", "Formats table (.dic) files into a readable, class-hierarchy format. Supports Diffmark.");
		}

		public static void Print()
		{
			Console.WriteLine();
			Console.WriteLine("Rant Command Line Tools");
			Console.WriteLine($"\n  Rant Version: {typeof(RantEngine).Assembly.GetName().Version.ToString(3)}");
			Console.WriteLine();
			Console.WriteLine("Usage: rant <command> [args] [options]");
			Console.WriteLine("\nAvailable options:");
			Console.WriteLine("  --version\t\t\tPrints the Rant version number.");
			Console.WriteLine("  --help\t\t\tDisplays Rant help.");
			Console.WriteLine("\nAvailable commands:");
			foreach (var item in Items.Values)
			{
				Console.WriteLine($"  {item.Name}\t\t\t{item.Description}");
			}
		}

		private static void AddHelpItem(string name, string desc)
		{
			Items[name] = new HelpItem(name, desc);
		}

		private class HelpItem
		{
			public readonly string Description;
			public readonly string Name;

			public HelpItem(string name, string desc)
			{
				Name = name;
				Description = desc;
			}

			public override string ToString() => $"{Name}: {Description}";
		}
	}
}