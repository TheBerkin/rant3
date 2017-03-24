#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Rant.Core.IO.Bson;
using Rant.Resources;
using Rant.Tools.Packer;
using Rant.Vocabulary;

namespace Rant.Tools.Commands
{
    [CommandName("pack", Description = "Packages resources in a specified directory into a .rantpkg archive.", UsesPath = true)]
    [CommandParam("out", false, "Output path for package.")]
    [CommandParam("version", false, "Overrides the package version string in rantpkg.json.")]
    [CommandFlag("no-compress", "Indicates that the package content should not be compressed.")]
    internal sealed class PackCommand : Command
    {
        public PackCommand()
        {
            
        }

        protected override void OnRun()
        {
            var pkg = new RantPackage();
            var paths = CmdLine.GetPaths();
            bool compress = !CmdLine.Flag("no-compress");
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

            // -out property overrides rantpkg.json's output path
            if (!string.IsNullOrWhiteSpace(CmdLine.Property("out")))
            {
                outputPath = Path.Combine(CmdLine.Property("out"), $"{pkg.ID}-{pkg.Version}.rantpkg");
                Directory.CreateDirectory(CmdLine.Property("out"));
            }
            // Otherwise, use rantpkg.json's "out" property
            else if (!string.IsNullOrWhiteSpace(info.OutputPath))
            {
                outputPath = Path.Combine(contentDir, info.OutputPath, $"{pkg.ID}-{pkg.Version}.rantpkg");
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }
            // If it doesn't have one, put it in the package content directory
            else
            {
                outputPath = Path.Combine(Directory.GetParent(contentDir).FullName, $"{pkg.ID}-{pkg.Version}.rantpkg");
            }

            Console.WriteLine($"String table mode: {modeEnum}");
            Console.WriteLine($"Compression: {(compress ? "yes" : "no")}");

            Console.WriteLine(compress ? "Compressing and saving..." : "Saving...");
            pkg.Save(outputPath, compress, modeEnum);

            Console.WriteLine("\nPackage saved to " + outputPath.Replace('\\', '/'));

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

            foreach (string path in Directory.GetFiles(contentPath, "*.table", SearchOption.AllDirectories))
            {
                Console.WriteLine("+ " + path);
                var table = RantDictionaryTable.FromStream(Path.GetFileNameWithoutExtension(path), File.Open(path, FileMode.Open));
                package.AddResource(table);
            }
        }

        private static bool TryGetRelativePath(string rootDir, string fullPath, out string relativePath, bool removeExtension = false)
        {
            relativePath = null;
            if (string.IsNullOrWhiteSpace(rootDir) || string.IsNullOrWhiteSpace(fullPath)) return false;
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