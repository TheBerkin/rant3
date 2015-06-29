using System;
using System.Collections.Generic;

using static System.Console;

namespace Rave
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
			WriteLine("Rave - Command-line utilities for Rant");
			WriteLine("Usage: rave <command> [args] [options]");
			WriteLine("Type \"rave help <command-name>\" to see help text for a specific command.\n");
			WriteLine("Available Commands:\n");
			foreach (var item in Items.Values)
			{
				WriteLine(item);
			}
		}

		private static void AddHelpItem(string name, string desc)
		{
			Items[name] = new HelpItem(name, desc);
		}

		private class HelpItem
		{
			public readonly string Name;
			public readonly string Description;

			public HelpItem(string name, string desc)
			{
				Name = name;
				Description = desc;
			}

			public override string ToString() => $"{Name}: {Description}\n";
		}
	}
}