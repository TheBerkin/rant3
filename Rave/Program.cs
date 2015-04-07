using System;
using static System.Console;
using static Rave.CmdLine;
using Rave.DicSort;
using Rave.Packer;
using Rave.DicDoc;

namespace Rave
{
	class Program
	{
		static void Main(string[] args)
		{
			if (String.IsNullOrEmpty(Command))
			{
				Help.Print();
				return;
			}

			switch (Command)
			{
				case "docs":
					{
						DocGenerator.Run();
						break;
					}
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
						foreach (var name in GetPaths())
						{
							WriteLine($"'{name}'");

							switch (name.ToLower())
							{
								case "docs":
									DocGenerator.GetHelp();
									break;
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
					WriteLine($"Unknown command: '{Command}'");
					break;
			}
		}
	}
}
