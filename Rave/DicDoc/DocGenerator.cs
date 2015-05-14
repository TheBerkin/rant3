using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

using Rant.Vocabulary;

namespace Rave.DicDoc
{
	public static class DocGenerator
	{
		public static void GetHelp()
		{
			Console.WriteLine("USAGE\n");

			Console.WriteLine("  rave docs");
			Console.WriteLine("    - Generates documentation for the current directory.");
			Console.WriteLine("  rave docs your-dictionary-dir");
			Console.WriteLine("    - Generates documentation for the specified directory.");
		}

		public static void Run()
		{
			Console.WriteLine("Working...");

			var args = Environment.GetCommandLineArgs().Skip(2).ToArray();
			var dicDir = args.Length == 0 ? Environment.CurrentDirectory : args[0];

			GenerateDictionary(dicDir);

			Console.WriteLine("Done.");
		}

		static void GenerateDictionary(string dicDir)
		{
			Console.WriteLine("Loading tables...");
			var tablePaths = Directory.GetFiles(dicDir, "*.dic");
			var tables = tablePaths.Select(RantDictionaryTable.FromFile).ToArray();

			if (tables.Length == 0)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("No tables found!");
				Console.ResetColor();
				return;
			}

			int totalEntries = tables.Sum(table => table.GetEntries().Count());

			Console.WriteLine("Creating directory structure...");

			// Destination directory
			var rootDir = Path.Combine(dicDir, "dicdoc");
			Mkdir(rootDir);

			// Documentation directory
			var entriesDir = Path.Combine(rootDir, "entries");
			Mkdir(entriesDir);


			File.Copy(Path.Combine(Util.BaseDir, "res/dicdoc.css"), rootDir + "/dicdoc.css");

			var text = new StringWriter();

			using (var writer = new HtmlTextWriter(text))
			{
				writer.WriteLine("<!DOCTYPE html>");

				writer.RenderBeginTag(HtmlTextWriterTag.Html);

				// Header
				writer.RenderBeginTag(HtmlTextWriterTag.Head);

				// Title
				writer.RenderBeginTag(HtmlTextWriterTag.Title);
				writer.WriteEncodedText("Documentation Home");
				writer.RenderEndTag();

				// Stylesheet
				writer.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
				writer.AddAttribute(HtmlTextWriterAttribute.Href, "dicdoc.css");
				writer.RenderBeginTag(HtmlTextWriterTag.Link);
				writer.RenderEndTag();

				writer.RenderEndTag(); // </head>

				writer.RenderBeginTag(HtmlTextWriterTag.Body);

				// Header
				writer.RenderBeginTag(HtmlTextWriterTag.H1);
				writer.WriteEncodedText("Home");
				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.P);

				writer.WriteEncodedText("The dictionary contains "
					+ tables.Length + (tables.Length == 1 ? " table" : " tables")
					+ " with a total of " + totalEntries + (totalEntries == 1 ? " entry" : " entries")
					+ ".");

				writer.RenderEndTag(); // </p>

				writer.RenderBeginTag(HtmlTextWriterTag.H2);
				writer.WriteEncodedText("Browse");
				writer.RenderEndTag(); // </h2>

				writer.RenderBeginTag(HtmlTextWriterTag.Ul);

				for (int i = 0; i < tables.Length; i++)
				{
					writer.RenderBeginTag(HtmlTextWriterTag.Li);

					writer.AddAttribute(HtmlTextWriterAttribute.Href, "table-" + tables[i].Name + ".html");
					writer.RenderBeginTag(HtmlTextWriterTag.A);
					writer.WriteEncodedText("<" + tables[i].Name + ">");
					writer.RenderEndTag(); // </a>
					writer.WriteEncodedText(" (" + Path.GetFileName(tablePaths[i]) + ")");

					writer.RenderEndTag(); // </li>
				}

				writer.RenderEndTag(); // </ul>

				writer.RenderEndTag(); // </body>

				writer.RenderEndTag(); // </html>
			}

			File.WriteAllText(Path.Combine(rootDir, "index.html"), text.ToString());

			Console.WriteLine("Generating documentation...");
			for (int i = 0; i < tables.Length; i++)
			{
				GenerateTable(tablePaths[i], tables[i], rootDir, entriesDir);
			}
		}

		static void GenerateTable(string tablePath, RantDictionaryTable table, string rootDir, string entriesDir)
		{
			var tableClasses = new HashSet<string>();
			foreach (var entry in table.GetEntries())
			{
				foreach (var entryClass in entry.GetClasses())
				{
					tableClasses.Add(entryClass);
				}
			}

			File.WriteAllText(rootDir + "/" + "table-" + table.Name + ".html", PageGenerator.GenerateTablePage(table, tablePath));

			foreach (var tableClass in tableClasses)
			{
				File.WriteAllText(entriesDir + "/" + table.Name + "-" + tableClass + ".html", PageGenerator.GenerateTableClassPage(table, tableClass));
			}

			File.WriteAllText(entriesDir + "/" + table.Name + ".html", PageGenerator.GenerateTableClassPage(table, ""));
		}

		static void Mkdir(string dir)
		{
			if (Directory.Exists(dir)) Directory.Delete(dir, true);

			Directory.CreateDirectory(dir);
		}
	}
}