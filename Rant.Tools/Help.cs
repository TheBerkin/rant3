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
			AddHelpItem("help", "Displays help text for a command.");
			AddHelpItem("pack", "Packages patterns and dictionaries into a portable archive format that can be loaded directly into Rant.");
			AddHelpItem("sort", "Formats table (.dic) files into a readable, class-hierarchy format. Supports Diffmark.");
		}

		public static void Print()
		{
			Console.WriteLine();
			Console.WriteLine("Rant Procedural Text Generation Engine");
			Console.WriteLine($"\n  Version: {typeof(RantEngine).Assembly.GetName().Version}");
			Console.WriteLine();
			Console.WriteLine("Usage: rant <command> [args] [options]");
			Console.WriteLine();
			Console.WriteLine("Type \"rant help <command-name>\" to see help text for a specific command.\n");
			Console.WriteLine("Available commands:");
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