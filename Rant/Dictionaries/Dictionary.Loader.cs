using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant.Dictionaries
{
    public sealed partial class Dictionary
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
        /// Loads a WordList from the file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file to load.</param>
        /// <param name="nsfwFilter">Specifies whether to allow or disallow NSFW entries.</param>
        /// <returns></returns>
        public static Dictionary FromFile(string path, NsfwFilter nsfwFilter = NsfwFilter.Disallow)
        {
            using (var reader = new StreamReader(path))
            {
                string name = null;
                string[] subs = null;
                var words = new List<DictionaryEntry>();

                bool any = false; // Prevent multiple Any() calls
                bool entryBlock = false;
                var currentEntries = new List<string>();
                var currentClasses = new List<string>();
                var currentWeight = 1;
                bool nsfw = false;
                bool endNsfw = false;

                while (!reader.EndOfStream)
                {
                    var entry = ReadEntry(reader);
                    if (entry == null) continue;
                    switch (entry.Item1.ToLower())
                    {
                        case "nsfw":
                            nsfw = true;
                            break;
                        case "endnsfw":
                            endNsfw = true;
                            break;
                        case "name":
                            if (name != null)
                            {
                                throw new InvalidDataException("Multiple #name declarations in dictionary file.");
                            }

                            name = entry.Item2;
                            break;
                        case "subtypes":
                            if (subs != null)
                            {
                                throw new InvalidDataException("Multiple subtype declarations in dictionary file.");
                            }

                            subs = entry.Item2.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            break;
                        case "one":
                            if (subs != null)
                            {
                                throw new InvalidDataException("Multiple subtype declarations in dictionary file.");
                            }

                            subs = new[] { "default" };
                            break;
                        case "entry":
                            if (name == null || subs == null)
                            {
                                throw new InvalidDataException("Missing name or subtype declarations before entry list.");
                            }

                            if (currentEntries.Count > 0)
                            {
                                if ((nsfwFilter == NsfwFilter.Disallow && !nsfw) || nsfwFilter == NsfwFilter.Allow)
                                {
                                    words.Add(new DictionaryEntry(currentEntries.ToArray(), currentClasses.ToArray(),
                                        currentWeight));

                                }
                                any = true;
                            }
                            else if (any)
                            {
                                throw new InvalidDataException("Attempted to add empty dictionary entry.");
                            }

                            if (endNsfw)
                            {
                                nsfw = false;
                                endNsfw = false;
                            }

                            currentEntries = new List<string>();
                            currentClasses = new List<string>();
                            currentWeight = 1;
                            entryBlock = true;
                            break;
                        case "values":
                            if (!entryBlock)
                            {
                                throw new InvalidDataException("Entry values defined before any #entry scope.");
                            }

                            currentEntries.AddRange(entry.Item2.Split(',').Select(s => s.Trim()));
                            break;
                        case "classes":
                            if (!entryBlock)
                            {
                                throw new InvalidDataException("Entry classes defined before any #entry scope.");
                            }

                            currentClasses.AddRange(entry.Item2.Split(',').Select(s => s.Trim().ToLower()));
                            break;
                        case "weight":
                            if (!entryBlock)
                            {
                                throw new InvalidDataException("Entry weight defined before any #entry scope.");
                            }
                            int weight;
                            if (!Int32.TryParse(entry.Item2.Trim(), out weight))
                            {
                                weight = 1;
                            }

                            currentWeight = weight;
                            break;
                        default:
                            continue;
                    }
                }

                if (name == null || subs == null)
                {
                    throw new InvalidDataException("Missing name or subtype declarations.");
                }

                if (currentEntries.Count > 0)
                {
                    words.Add(new DictionaryEntry(currentEntries.ToArray(), currentClasses.ToArray(), currentWeight));
                    any = true;
                }

                if (!any)
                {
                    throw new InvalidDataException("No entries in dictionary.");
                }

                return new Dictionary(name, subs, words.ToArray());
            }
        }
    }
}