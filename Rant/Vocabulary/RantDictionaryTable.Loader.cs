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
        /// <param name="nsfwFilter">Specifies whether to allow or disallow NSFW entries.</param>
        /// <returns></returns>
        public static RantDictionaryTable FromFile(string path, NsfwFilter nsfwFilter = NsfwFilter.Disallow)
        {
            var name = "";
            string[] subtypes = { "default" };
            bool header = true;
            bool nsfw = false;

            var scopedClassSet = new HashSet<string>();
            RantDictionaryEntry entry = null;
            var entries = new List<RantDictionaryEntry>();

            foreach (var token in DicLexer.Tokenize(path, File.ReadAllText(path)))
            {
                switch (token.ID)
                {
                    case DicTokenType.Directive:
                        {
                            var parts = token.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (!parts.Any()) continue;

                            switch (parts[0].ToLower())
                            {
                                case "name":
                                    if (!header) LoadError(path, token, "The #name directive may only be used in the file header.");
                                    if (parts.Length != 2) LoadError(path, token, "#name directive expected one word:\r\n\r\n" + token.Value);
                                    if (!Util.ValidateName(parts[1])) LoadError(path, token, $"Invalid #name value: '{parts[1]}'");
                                    name = parts[1].ToLower();
                                    break;
                                case "subs":
                                    if (!header) LoadError(path, token, "The #subs directive may only be used in the file header.");
                                    subtypes = parts.Skip(1).Select(s => s.Trim().ToLower()).ToArray();
                                    break;
                                case "version": // Kept here for backwards-compatability
                                    if (!header) LoadError(path, token, "The #version directive may only be used in the file header.");
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
                                                    scopedClassSet.Add(cl.ToLower());
                                                break;
                                            case "remove":
                                                foreach (var cl in parts.Skip(2))
                                                    scopedClassSet.Remove(cl.ToLower());
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case DicTokenType.Entry:
                        {
                            if (nsfwFilter == NsfwFilter.Disallow && nsfw) continue;
                            if (Util.IsNullOrWhiteSpace(name))
                                LoadError(path, token, "Missing dictionary name before entry list.");
                            if (Util.IsNullOrWhiteSpace(token.Value))
                                LoadError(path, token, "Encountered empty dictionary entry.");
                            header = false;
                            entry = new RantDictionaryEntry(token.Value.Split('/').Select(s => s.Trim()).ToArray(), scopedClassSet, nsfw);
                            entries.Add(entry);
                        }
                        break;
                    case DicTokenType.DiffEntry:
                        {
                            if (nsfwFilter == NsfwFilter.Disallow && nsfw) continue;
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
                                }).ToArray(), scopedClassSet, nsfw);
                            entries.Add(entry);
                        }
                        break;
                    case DicTokenType.Property:
                        {
                            if (nsfwFilter == NsfwFilter.Disallow && nsfw) continue;
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
                                        if (subtypes.Length != pron.Length)
                                            LoadError(path, token, "Pronunciation list length must match subtype count.");

                                        for (int i = 0; i < entry.Terms.Length; i++)
                                            entry.Terms[i].Pronunciation = pron[i];
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            return new RantDictionaryTable(name, subtypes, entries);
        }

        private static void LoadError(string file, Stringe data, string message)
        {
            throw new InvalidDataException($"({Path.GetFileName(file)}, Line {data.Line}): {message}");
        }
    }
}