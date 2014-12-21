using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Stores a Rant dictionary.
    /// </summary>
    public sealed partial class RantDictionaryTable
    {
        /// <summary>
        /// The current version number of the dictionary format.
        /// </summary>
        public const string Version = "2";

        private readonly string _name;
        private readonly string[] _subtypes;
        private readonly RantDictionaryEntry[] _words;

        /// <summary>
        /// Creates a new WordList from the specified data.
        /// </summary>
        /// <param name="name">the name of the list.</param>
        /// <param name="subtypes">The subtype names.</param>
        /// <param name="words">The words to add to the list.</param>
        public RantDictionaryTable(string name, string[] subtypes, IEnumerable<RantDictionaryEntry> words)
        {
            if (!Util.ValidateName(name))
            {
                throw new FormatException("Invalid dictionary name: '\{name}'");
            }

            if (!subtypes.All(Util.ValidateName))
            {
                throw new FormatException("Invalid subtype name(s): " + String.Join(", ", subtypes.Where(s => !Util.ValidateName(s)).Select(s => "'\{s}'")));
            }

            _subtypes = subtypes;
            _name = name;
            _words = words.ToArray();
        }
        
        /// <summary>
        /// The entries stored in the dictionary.
        /// </summary>
        public RantDictionaryEntry[] Entries => _words;

        /// <summary>
        /// The subtypes in the dictionary.
        /// </summary>
        public string[] Subtypes => _subtypes;

        /// <summary>
        /// The name of the dictionary.
        /// </summary>
        public string Name => _name;

        private int GetSubtypeIndex(string subtype)
        {
            if (String.IsNullOrEmpty(subtype)) return 0;
            for (int i = 0; i < _subtypes.Length; i++)
            {
                if (String.Equals(subtype, _subtypes[i], StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        internal string Query(RNG rng, Query query, QueryState syncState)
        {
            var index = String.IsNullOrEmpty(query.Subtype) ? 0 : GetSubtypeIndex(query.Subtype);
            if (index == -1) return "[Missing Subtype]";

            IEnumerable<RantDictionaryEntry> pool = 
                query.Exclusive
                    ? _words.Where(e => e.GetClasses().Any() && e.GetClasses().All(
                        c => query.ClassFilters.Any(
                            set => set.Any(
                                t => t.Item2 == c))))
                    : _words.Where(
                        e => query.ClassFilters.All(
                            set => set.Any(
                                t => t.Item1 == e.ContainsClass(t.Item2))));

            pool = query.RegexFilters.Aggregate(pool, (current, regex) => current.Where(e => regex.Item1 == regex.Item2.IsMatch(e.Terms[index].Value)));

            if (!pool.Any())
            {
                return "[?]";
            }

            return syncState.GetEntry(query.Carrier, index, pool, rng)?[index] ?? "[?]";
        }
    }
}