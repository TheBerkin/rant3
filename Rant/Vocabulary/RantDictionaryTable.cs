using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine;

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

        internal const string MissingTerm = "[?]";

        private readonly string _name;
        private readonly string[] _subtypes;
        private readonly List<RantDictionaryEntry> _entries;

        /// <summary>
        /// Creates a new WordList from the specified data.
        /// </summary>
        /// <param name="name">the name of the list.</param>
        /// <param name="subtypes">The subtype names.</param>
        /// <param name="entries">The words to add to the list.</param>
        public RantDictionaryTable(string name, string[] subtypes, IEnumerable<RantDictionaryEntry> entries)
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
            _entries = entries.ToList();
        }
        
        /// <summary>
        /// Gets the entries stored in the table.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RantDictionaryEntry> GetEntries()
        {
            foreach (var entry in _entries) yield return entry;
        }

        /// <summary>
        /// The subtypes used by the table entries.
        /// </summary>
        public string[] Subtypes => _subtypes;

        /// <summary>
        /// The name of the table.
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

        /// <summary>
        /// Adds another table's entries to the current table, given that they share the same name and subtypes.
        /// </summary>
        /// <param name="other">The table whose entries will be added to the current instance.</param>
        /// <returns>True if merge succeeded; otherwise, False.</returns>
        public bool Merge(RantDictionaryTable other)
        {
            if (other._name != _name) return false;
            if (!other._subtypes.SequenceEqual(_subtypes)) return false;
            _entries.AddRange(other._entries);
            return true;
        }

        internal string Query(RNG rng, Query query, QueryState syncState)
        {
            var index = String.IsNullOrEmpty(query.Subtype) ? 0 : GetSubtypeIndex(query.Subtype);
            if (index == -1) return "[Bad Subtype]";

            IEnumerable<RantDictionaryEntry> pool = 
                query.Exclusive
                    ? _entries.Where(
                        e => e.GetClasses().Any() == query.ClassFilters.Any()
                        && e.GetClasses().All(
                            c => query.ClassFilters.Any(
                                set => set.Any(
                                    t => t.Item2 == c))))
                    : _entries.Where(
                        e => query.ClassFilters.All(
                            set => set.Any(
                                t => t.Item1 == e.ContainsClass(t.Item2))));

            pool = query.RegexFilters.Aggregate(pool, (current, regex) => current.Where(e => regex.Item1 == regex.Item2.IsMatch(e.Terms[index].Value)));

            if (query.SyllablePredicate != null)
                pool = pool.Where(e => query.SyllablePredicate(e.Terms[index].SyllableCount));

            if (!pool.Any()) return MissingTerm;

            return syncState.GetEntry(query.Carrier, index, pool, rng)?[index] ?? MissingTerm;
        }
    }
}