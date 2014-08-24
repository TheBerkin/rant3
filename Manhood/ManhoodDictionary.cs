using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Manhood
{
    /// <summary>
    /// Stores a Manhood dictionary.
    /// </summary>
    public sealed class ManhoodDictionary
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
        public ManhoodDictionary(string name, string[] subtypes, DictionaryEntry[] words)
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
            _words = words;
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
        public static ManhoodDictionary FromFile(string path, NsfwFilter nsfwFilter = NsfwFilter.Disallow)
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

                return new ManhoodDictionary(name, subs, words.ToArray());
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
            for (int i = 0; i < _subtypes.Length; i++)
            {
                if (String.Equals(subtype, _subtypes[i], StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        internal string GetWord(Interpreter interpreter, Query query)
        {
            const string NotFound = "(NOT FOUND)";

            int subtypeIndex = GetSubtypeIndex(query.Subtype);

            if (subtypeIndex == -1) throw new ManhoodException(String.Concat("Subtype '", query.Subtype, "' doesn't exist in dictionary '", Name, "'"));
            
            IEnumerable<DictionaryEntry> wordPool = _words;

            foreach (var filter in query.Filters)
            {
                var name = filter.Item1.ToLower().Trim();

                if (String.IsNullOrEmpty(name)) continue;

                var value = name.EndsWith("-pat")
                    ? interpreter.Evaluate(filter.Item2)
                    : filter.Item2;

                switch (new String(name.TakeWhile(c => c != '-').ToArray()))
                {
                    case "in":
                    {
                        wordPool = wordPool.Where(w => w.Classes.Contains(value));
                        continue;
                    }
                    case "onlyin":
                    {
                        wordPool = wordPool.Where(w => w.Classes.Count > 0 && !w.Classes.Except(value.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Any());
                        continue;
                    }
                    case "notin":
                    {
                        wordPool = wordPool.Where(w => !w.Classes.Contains(value));
                        continue;
                    }
                    case "for":
                    {
                        var hash = value.Hash();
                        return wordPool.Any()
                            ? wordPool.ElementAt(interpreter.State.RNG.PeekAt(hash, wordPool.Count()))
                                .Values[subtypeIndex]
                            : NotFound;
                    }
                    case "with":
                    {
                        var regex = new Regex(value, RegexOptions.IgnoreCase);
                        wordPool = wordPool.Where(w => regex.IsMatch(w.Values[subtypeIndex]));
                        continue;
                    }
                    case "without":
                    {
                        var regex = new Regex(value, RegexOptions.IgnoreCase);
                        wordPool = wordPool.Where(w => !regex.IsMatch(w.Values[subtypeIndex]));
                        continue;
                    }
                }
                
            }

            if (!wordPool.Any()) return NotFound;

            int selection = interpreter.State.RNG.Next(wordPool.Sum(w => w.Weight));

            foreach (var word in wordPool)
            {
                if (selection < word.Weight)
                {
                    return word.Values[subtypeIndex];
                }
                selection -= word.Weight;
            }

            return NotFound;
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