using System;

using Rant.Tools.DicSort;
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
					case "sort":
					{
						TableSorter.Run();
						break;
					}
					case "pack":
					{
						PackGenerator.Run();
						break;
					}
					case "build":
					{
						// TODO: compiler command
						WriteLine("Sorry, this command isn't implemented yet :(");
						break;
					}
					case "help":
					{
						foreach (string name in CmdLine.GetPaths())
						{
							WriteLine($"'{name}'\n");

							switch (name.ToLower())
							{
								case "sort":
									TableSorter.GetHelp();
									break;
								case "pack":
									PackGenerator.GetHelp();
									break;
								case "help":
									WriteLine("Are you serious?");
									break;
								default:
									WriteLine($"No help info found for '{name}'");
									break;
							}
							WriteLine();
						}
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
	}
}