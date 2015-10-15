using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant.Engine;
using Rant.Stringes;

namespace Rant.Vocabulary
{
    public sealed partial class RantDictionaryTable
    {
        /// <summary>
        /// Loads a RantDictionary from the file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file to load.</param>
        /// <returns></returns>
        public static RantDictionaryTable FromFile(string path)
        {
            var name = "";
            string[] subtypes = { "default" };
            bool header = true;

            var scopedClassSet = new HashSet<string>();
            RantDictionaryEntry entry = null;
            var entries = new List<RantDictionaryEntry>();
            var entryStringes = new List<Stringe>();
            var types = new Dictionary<string, EntryTypeDef>();
            var hiddenClasses = new HashSet<string> { "nsfw" };

            foreach (var token in DicLexer.Tokenize(path, File.ReadAllText(path)))
            {
                switch (token.ID)
                {
                    case DicTokenType.Directive:
                        {
                            var parts = VocabUtils.GetArgs(token.Value).ToArray();
                            if (!parts.Any()) continue;
                            var dirName = parts.First().ToLower();
                            var args = parts.Skip(1).ToArray();

                            switch (dirName)
                            {
                                case "name":
                                    if (!header) LoadError(path, token, "The #name directive may only be used in the file header.");
                                    if (args.Length != 1) LoadError(path, token, "#name directive expected one word:\r\n\r\n" + token.Value);
                                    if (!Util.ValidateName(args[0])) LoadError(path, token, $"Invalid #name value: '{args[1]}'");
                                    name = args[0].ToLower();
                                    break;
                                case "subs":
                                    if (!header) LoadError(path, token, "The #subs directive may only be used in the file header.");
                                    subtypes = args.Select(s => s.Trim().ToLower()).ToArray();
                                    break;
                                case "version": // Kept here for backwards-compatability
                                    if (!header) LoadError(path, token, "The #version directive may only be used in the file header.");
                                    break;
                                case "hidden":
                                    if (!header) LoadError(path, token, "The #hidden directive may only be used in the file header.");
                                    if (Util.ValidateName(args[0])) hiddenClasses.Add(args[0]);
                                    break;
                                case "nsfw":
                                    scopedClassSet.Add("nsfw");
                                    break;
                                case "sfw":
                                    scopedClassSet.Remove("nsfw");
                                    break;
                                case "class":
                                    {
                                        if (args.Length < 2) LoadError(path, token, "The #class directive expects an operation and at least one value.");
                                        switch (args[0].ToLower())
                                        {
                                            case "add":
                                                foreach (var cl in args.Skip(1))
                                                    scopedClassSet.Add(cl.ToLower());
                                                break;
                                            case "remove":
                                                foreach (var cl in args.Skip(1))
                                                    scopedClassSet.Remove(cl.ToLower());
                                                break;
                                        }
                                    }
                                    break;
                                case "type":
                                    {
                                        if (!header) LoadError(path, token, "The #type directive may only be used in the file header.");
                                        if (args.Length != 3) LoadError(path, token, "#type directive requires 3 arguments.");
                                        types.Add(args[0], new EntryTypeDef(args[0], args[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                                            Util.IsNullOrWhiteSpace(args[2]) ? null : new EntryTypeDefFilter(args[2])));
                                    }
                                    break;
                            }
                        }
                        break;
                    case DicTokenType.Entry:
                        {
                            if (Util.IsNullOrWhiteSpace(name))
                                LoadError(path, token, "Missing dictionary name before entry list.");
                            if (Util.IsNullOrWhiteSpace(token.Value))
                                LoadError(path, token, "Encountered empty dictionary entry.");
                            header = false;
                            entry = new RantDictionaryEntry(token.Value.Split('/').Select(s => s.Trim()).ToArray(), scopedClassSet);
                            entries.Add(entry);
                            entryStringes.Add(token);
                        }
                        break;
                    case DicTokenType.DiffEntry:
                        {
                            if (Util.IsNullOrWhiteSpace(name))
                                LoadError(path, token, "Missing dictionary name before entry list.");
                            if (Util.IsNullOrWhiteSpace(token.Value))
                                LoadError(path, token, "Encountered empty dictionary entry.");
                            header = false;
                            string first = null;
                            entry = new RantDictionaryEntry(token.Value.Split('/')
                                .Select((s, i) =>
                                {
                                    if (i > 0) return Diff.Mark(first, s);
                                    return first = s.Trim();
                                }).ToArray(), scopedClassSet);
                            entries.Add(entry);
                            entryStringes.Add(token);
                        }
                        break;
                    case DicTokenType.Property:
                        {
                            var parts = token.Value.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                            if (!parts.Any()) LoadError(path, token, "Empty property field.");
                            switch (parts[0].ToLower())
                            {
                                case "class":
                                    {
                                        if (parts.Length < 2) continue;
                                        foreach (var cl in parts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            bool opt = cl.EndsWith("?");
                                            entry.AddClass(VocabUtils.GetString(opt ? cl.Substring(0, cl.Length - 1) : cl), opt);
                                        }
                                    }
                                    break;
                                case "weight":
                                    {
                                        if (parts.Length != 2) LoadError(path, token, "'weight' property expected a value.");
                                        int weight;
                                        if (!Int32.TryParse(parts[1], out weight))
                                            LoadError(path, token, "Invalid weight value: '" + parts[1] + "'");
                                        entry.Weight = weight;
                                    }
                                    break;
                                case "pron":
                                    {
                                        if (parts.Length != 2) LoadError(path, token, "'" + parts[0] + "' property expected a value.");
                                        var pron =
                                            parts[1].Split('/')
                                                .Select(s => s.Trim())
                                                .ToArray();
                                        if (subtypes.Length == pron.Length)
                                        {
                                            for (int i = 0; i < entry.Terms.Length; i++)
                                                entry.Terms[i].Pronunciation = pron[i];
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        EntryTypeDef typeDef;
                                        if (!types.TryGetValue(parts[0], out typeDef))
                                            LoadError(path, token, $"Unknown property name '{parts[0]}'.");
                                        // Okay, it's a type.
                                        if (parts.Length != 2) LoadError(path, token, "Missing type value.");
                                        entry.AddClass(VocabUtils.GetString(parts[1]));
                                        if(!typeDef.IsValidValue(parts[1]))
                                            LoadError(path, token, $"'{parts[1]}' is not a valid value for type '{typeDef.Name}'.");
                                        break;
                                    }
                            }
                        }
                        break;
                }
            }

            if (types.Any())
            {
                var eEntries = entries.GetEnumerator();
                var eEntryStringes = entryStringes.GetEnumerator();
                while (eEntries.MoveNext() && eEntryStringes.MoveNext())
                {
                    foreach (var type in types.Values)
                    {
                        if (!type.Test(eEntries.Current))
                        {
                            // TODO: Find a way to output multiple non-fatal table load errors without making a gigantic exception message.
                            LoadError(path, eEntryStringes.Current, $"Entry '{eEntries.Current}' does not satisfy type '{type.Name}'.");
                        }
                    }
                }
            }

            return new RantDictionaryTable(name, subtypes, entries, hiddenClasses);
        }

        private static void LoadError(string file, Stringe data, string message)
        {
            throw new RantTableLoadException(file, data, message);
        }
    }
}