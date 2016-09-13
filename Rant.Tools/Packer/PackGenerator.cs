using System;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Rant.Core.IO.Bson;
using Rant.Resources;

namespace Rant.Tools.Packer
{
	public static class PackGenerator
	{
		public static void GetHelp()
		{
			Console.WriteLine("Usage:");

			Console.WriteLine("  rant pack <-out package-path>");
			Console.WriteLine("    - Creates a package from the current directory.");
			Console.WriteLine("      -out: Specifies the output path for the package. Optional if rantpkg.json specifies an output path.");
			Console.WriteLine("  rant pack [content-dir] <-out package-path>");
			Console.WriteLine("    - Creates a package from the specified directories.");
			Console.WriteLine("      -out: Specifies the output path for the package. Optional if rantpkg.json specifies an output path.");
			Console.WriteLine("  Options:");
			Console.WriteLine("    --compression [true|false]: Enable or disable LZMA compression. Defaults to true.");
			Console.WriteLine("    -string-table [mode]: Set the string table mode. 0 = none, 1 = keys, 2 = keys and values.");
			Console.WriteLine("                          Defaults to keys.");
			Console.WriteLine("    -version [version]: Overrides the version in rantpkg.json.");
		}

		public static void Run()
		{
			var pkg = new RantPackage();
			var paths = CmdLine.GetPaths();
			bool compress = CmdLine.Property("compression", "true") == "true";
			int stringTableMode = int.Parse(CmdLine.Property("string-table", "1"));

			if (stringTableMode < 0 || stringTableMode > 2)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Invalid string table mode.");
				Console.ResetColor();
				return;
			}

			var modeEnum = (BsonStringTableMode)stringTableMode;

			Console.WriteLine("Packing...");

			string contentDir = Path.GetFullPath(paths.Length == 0 ? Environment.CurrentDirectory : paths[0]);

			Pack(pkg, contentDir);

			string outputPath;
			string infoPath = Path.Combine(contentDir, "rantpkg.json");
			if (!File.Exists(infoPath))
				throw new FileNotFoundException("rantpkg.json missing from root directory.");

			var info = JsonConvert.DeserializeObject<PackInfo>(File.ReadAllText(infoPath));
			pkg.Title = info.Title;
			pkg.Authors = info.Authors;
			pkg.Version = RantPackageVersion.Parse(!string.IsNullOrWhiteSpace(CmdLine.Property("version")) ? CmdLine.Property("version") : info.Version);
			pkg.Description = info.Description;
			pkg.ID = info.ID;
			pkg.Tags = info.Tags;
			foreach (var dep in info.Dependencies)
			{
				pkg.AddDependency(dep);
			}

			if (!string.IsNullOrWhiteSpace(info.OutputPath))
			{
				outputPath = Path.Combine(contentDir, info.OutputPath, $"{pkg.ID}-{pkg.Version}.rantpkg");
				Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			}
			else
			{
				outputPath = Path.Combine(Directory.GetParent(contentDir).FullName, $"{pkg.ID}-{pkg.Version}.rantpkg");
			}

			Console.WriteLine($"String table mode: {modeEnum}");
			Console.WriteLine($"Compression: {(compress ? "yes" : "no")}");

			Console.WriteLine(compress ? "Compressing and saving..." : "Saving...");
			pkg.Save(outputPath, compress, modeEnum);

			Console.WriteLine("\nPackage saved to " + outputPath);

			Console.ResetColor();
		}

		private static void Pack(RantPackage package, string contentPath)
		{
			foreach (string path in Directory.EnumerateFiles(contentPath, "*.*", SearchOption.AllDirectories)
				.Where(p => p.EndsWith(".rant") || p.EndsWith(".rants")))
			{
				var pattern = RantProgram.CompileFile(path);
				string relativePath;
				TryGetRelativePath(contentPath, path, out relativePath, true);
				pattern.Name = relativePath;
				package.AddResource(pattern);
				Console.WriteLine("+ " + pattern.Name);
			}

			foreach (string path in Directory.GetFiles(contentPath, "*.dic", SearchOption.AllDirectories))
			{
				throw new NotImplementedException(); // TODO
				//Console.WriteLine("+ " + path);
				//var table = RantDictionaryTable.FromFile(path);
				//package.AddTable(table);
			}
		}

		private static bool TryGetRelativePath(string rootDir, string fullPath, out string relativePath, bool removeExtension = false)
		{
			relativePath = null;
			if (string.IsNullOrWhiteSpace(rootDir)) return false;
			if (string.IsNullOrWhiteSpace(fullPath)) return false;
			var rootParts = Path.GetFullPath(rootDir).Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			var fullParts = Path.GetFullPath(fullPath).Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			if (rootParts.Length == 0 || fullParts.Length <= rootParts.Length)
			{
				relativePath = fullPath;
				return true;
			}
			for (int i = 0; i < rootParts.Length; i++)
			{
				if (rootParts[i] != fullParts[i]) return false;
			}
			var sb = new StringBuilder();
			int indDot;
			for (int j = rootParts.Length; j < fullParts.Length; j++)
			{
				if (j > rootParts.Length) sb.Append('/');
				if (removeExtension && j == fullParts.Length - 1 && (indDot = fullParts[j].LastIndexOf('.')) > -1)
				{
					sb.Append(fullParts[j].Substring(0, indDot));
				}
				else
				{
					sb.Append(fullParts[j]);
				}
			}
			relativePath = sb.ToString();
			return true;
		}
	}
}