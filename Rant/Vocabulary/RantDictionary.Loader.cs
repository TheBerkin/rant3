using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Rant.Stringes;

namespace Rant.Vocabulary
{
    public sealed partial class RantDictionary
    {
        private static Tuple<string, string> ReadEntry(StreamReader reader)
        {
            var line = reader.ReadLine();
            if (line == null) return null;
            var match = Regex.Match(line.Trim(), @"^\s*#(?<name>[\w_\-]+)(\s+(?<value>.*)\s*)?", RegexOptions.ExplicitCapture);
            if (!match.Success) return null;
            var groups = match.Groups;
            return Tuple.Create(groups["name"].Value, groups["value"].Value);
        }

        /// <summary>
        /// Loads a RantDictionary from the file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file to load.</param>
        /// <param name="nsfwFilter">Specifies whether to allow or disallow NSFW entries.</param>
        /// <returns></returns>
        public static RantDictionary FromFile(string path, NsfwFilter nsfwFilter = NsfwFilter.Disallow)
        {
            var name = "";
            var version = "2";
            string[] subtypes = {"default"};

            bool header = true;

            bool nsfw = false;

            var scopedClassSet = new HashSet<string>();

            RantDictionaryEntry entry = null;

            var entries = new List<RantDictionaryEntry>();

            foreach (var token in Dic2Lexer.Tokenize(File.ReadAllText(path)))
            {
                switch (token.Identifier)
                {
                    case DicTokenType.Directive:
                    {
                        var parts = token.Value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        if (!parts.Any()) continue;

                        switch (parts[0].ToLower())
                        {
                            case "name":
                                if (!header) LoadError(path, token, "The #name directive may only be used in the file header.");
                                if (parts.Length != 2) LoadError(path, token, "#name directive expected one word:\r\n\r\n" + token.Value);
                                if (!Util.ValidateName(parts[1])) LoadError(path, token, "Invalid #name value: '" + parts[1] + "'");
                                name = parts[1].ToLower();
                                break;
                            case "subs":
                                if (!header) LoadError(path, token, "The #subs directive may only be used in the file header.");
                                subtypes = parts.Skip(1).Select(s => s.Trim().ToLower()).ToArray();
                                break;
                            case "version":
                                if (!header) LoadError(path, token, "The #version directive may only be used in the file header.");
                                if (parts.Length != 2) LoadError(path, token, "The #version directive requires a value.");
                                version = parts[1];
                                if (version != "2")
                                {
                                    LoadError(path, token, "Unsupported file version '" + version + "'");
                                }
                                break;
                            case "nsfw":
                                nsfw = true;
                                break;
                            case "sfw":
                                nsfw = false;
                                break;
                                case "class":
                                {
                                    if (parts.Length < 3) LoadError(path, token, "The #class directive expects an operation and at least one value.");
                                    switch (parts[1].ToLower())
                                    {
                                        case "add":
                                            foreach (var cl in parts.Skip(2))
                                            {
                                                scopedClassSet.Add(cl.ToLower());
                                            }
                                            break;
                                        case "remove":
                                            foreach (var cl in parts.Skip(2))
                                            {
                                                scopedClassSet.Remove(cl.ToLower());
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                    case DicTokenType.Entry:
                    {
                        if (String.IsNullOrWhiteSpace(name)) LoadError(path, token, "Missing dictionary name before entry list.");
                        if (String.IsNullOrWhiteSpace(token.Value))
                        {
                            LoadError(path, token, "Encountered empty dictionary entry.");
                        }
                        header = false;
                        entry = new RantDictionaryEntry(token.Value.Split(new[] { '/' }).Select(s => s.Trim()).ToArray(), scopedClassSet, nsfw);
                        entries.Add(entry);
                    }
                    break;
                    case DicTokenType.Property:
                    {
                        var parts = token.Value.Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries);
                        if(!parts.Any()) LoadError(path, token, "Empty property field.");
                        switch (parts[0].ToLower())
                        {
                            case "class":
                            {
                                if (parts.Length < 2) continue;
                                foreach (var cl in parts[1].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    entry.Classes.Add(cl.ToLower());
                                }
                            }
                            break;
                            case "weight":
                            {
                                if (parts.Length != 2) LoadError(path, token, "'weight' property expected a value.");
                                int weight;
                                if (!Int32.TryParse(parts[1], out weight))
                                {
                                    LoadError(path, token, "Invalid weight value: '" + parts[1] + "'");
                                }
                                entry.Weight = weight;
                            }
                            break;
                            case "ipa":
                            case "pron":
                            {
                                if (parts.Length != 2) LoadError(path, token, "'" + parts[0] + "' property expected a value.");
                                entry.IPA =
                                    parts[1].Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .ToArray();
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return new RantDictionary(name, subtypes, entries);
        }

        private static void LoadError(string file, Stringe data, string message)
        {
            throw new InvalidDataException(String.Format("({0}, Line {1}): {2}", Path.GetFileName(file), data.Line, message));
        }
    }
}