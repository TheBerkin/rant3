using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant
{
    /// <summary>
    /// Stores a Rant dictionary.
    /// </summary>
    public sealed class Dictionary
    {
        private readonly string _name;
        private readonly string[] _subtypes;
        private readonly DictionaryEntry[] _words;
        private readonly Dictionary<string, List<DictionaryEntry>> _classIndices = new Dictionary<string, List<DictionaryEntry>>();

        /// <summary>
        /// Creates a new WordList from the specified data.
        /// </summary>
        /// <param name="name">the name of the list.</param>
        /// <param name="subtypes">The subtype names.</param>
        /// <param name="words">The words to add to the list.</param>
        public Dictionary(string name, string[] subtypes, IEnumerable<DictionaryEntry> words)
        {
            if (!Util.ValidateName(name))
            {
                throw new FormatException("Invalid word list name.");
            }
            if (!subtypes.All(Util.ValidateName))
            {
                throw new FormatException("Invalid subtype name(s).");
            }

            _subtypes = subtypes;
            _name = name;
            _words = words.ToArray();
            GenerateClassIndices();
        }

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

                            subs = entry.Item2.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                            break;
                        case "one":
                            if (subs != null)
                            {
                                throw new InvalidDataException("Multiple subtype declarations in dictionary file.");
                            }

                            subs = new[] {"default"};
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

        /// <summary>
        /// The name of the dictionary.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        private int GetSubtypeIndex(string subtype)
        {
            if (String.IsNullOrEmpty(subtype)) return 0;
            bool wStart = subtype.StartsWith("*");
            bool wEnd = subtype.EndsWith("*");
            subtype = subtype.Trim('*');
            for (int i = 0; i < _subtypes.Length; i++)
            {
                if (wStart && wEnd)
                {
                    if (_subtypes[i].IndexOf(subtype, StringComparison.OrdinalIgnoreCase) != -1) return i;
                }
                else if (wStart)
                {
                    if (_subtypes[i].EndsWith(subtype, StringComparison.OrdinalIgnoreCase)) return i;
                }
                else if (wEnd)
                {
                    if (_subtypes[i].StartsWith(subtype, StringComparison.OrdinalIgnoreCase)) return i;
                }
                if (String.Equals(subtype, _subtypes[i], StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        internal string Query(RNG rng, Query query)
        {
            var index = String.IsNullOrEmpty(query.Subtype) ? 0 : GetSubtypeIndex(query.Subtype);
            if (index == -1)
            {
                return "BAD_SUBTYPE";
            }

            IEnumerable<DictionaryEntry> pool = _words;

            pool = query.Exclusive
                ? pool.Where(e => e.Classes.Any() && e.Classes.All(c => query.ClassFilters.Any(set => set.Any(t => t.Item2 == c))))
                : pool.Where(e => query.ClassFilters.All(set => set.Any(t => t.Item1 == (e.Classes.Contains(t.Item2)))));

            pool = query.RegexFilters.Aggregate(pool, (current, regex) => current.Where(e => regex.Item1 == regex.Item2.IsMatch(e.Values[index])));

            if (!pool.Any())
            {
                return "NOT_FOUND";
            }

            int number = String.IsNullOrEmpty(query.Carrier) ? rng.Next(pool.Sum(e => e.Weight)) + 1
                : rng.PeekAt(query.Carrier.Hash(), pool.Sum(e => e.Weight));

            foreach (var e in pool)
            {
                if (number <= e.Weight) return e.Values[index];
                number -= e.Weight;
            }

            return "NOT_FOUND";
        }

        private List<DictionaryEntry> GetClassIndexList(string className)
        {
            List<DictionaryEntry> list;
            if (!_classIndices.TryGetValue(className = className.ToLower(), out list))
            {
                _classIndices[className] = list = new List<DictionaryEntry>();
            }
            return list;
        }

        private void GenerateClassIndices()
        {
            foreach (var word in _words)
            {
                foreach (var className in word.Classes)
                {
                    GetClassIndexList(className).Add(word);
                }
            }
        }
    }
}