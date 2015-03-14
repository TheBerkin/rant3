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
        public const int Version = 3;

        internal const string MissingTerm = "[?]";

        private readonly string _name;
        private readonly string[] _subtypes;
        private RantDictionaryEntry[] _entries;

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
                throw new FormatException("Invalid subtype name(s): " + String.Join(", ", subtypes.Where(s => !Util.ValidateName(s)).Select(s => "'\{s}'").ToArray()));
            }

            _subtypes = subtypes;
            _name = name;
            _entries = entries.ToArray();
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
        /// <param name="mergeBehavior">The merging strategy to employ.</param>
        /// <returns>True if merge succeeded; otherwise, False.</returns>
        public bool Merge(RantDictionaryTable other, TableMergeBehavior mergeBehavior = TableMergeBehavior.Naive)
        {
            if (other._name != _name || other == this) return false;
            if (!other._subtypes.SequenceEqual(_subtypes)) return false;
            switch (mergeBehavior)
            {
                case TableMergeBehavior.Naive:
                    Array.Resize(ref _entries, _entries.Length + other._entries.Length);
                    Array.Copy(other._entries, 0, _entries, other._entries.Length, other._entries.Length);
                    break;
                case TableMergeBehavior.RemoveEntryDuplicates: // TODO: Make this NOT O(n^2*subtypes) -- speed up with HashSet?
                    {
                        var otherEntries =
                            other._entries.Where(e => !_entries.Any(ee => ee.Terms.SequenceEqual(e.Terms))).ToArray();
                        Array.Resize(ref _entries, _entries.Length + otherEntries.Length);
                        Array.Copy(otherEntries, 0, _entries, otherEntries.Length, otherEntries.Length);
                        break;
                    }
                case TableMergeBehavior.RemoveFirstTermDuplicates: // TODO: Make this NOT O(n^2)
                    {
                        var otherEntries =
                            other._entries.Where(e => _entries.All(ee => ee.Terms[0] != e.Terms[0])).ToArray();
                        Array.Resize(ref _entries, _entries.Length + otherEntries.Length);
                        Array.Copy(otherEntries, 0, _entries, otherEntries.Length, otherEntries.Length);
                        break;
                    }
            }

            return true;
        }

        internal string Query(RNG rng, Query query, QueryState syncState)
        {
            var index = String.IsNullOrEmpty(query.Subtype) ? 0 : GetSubtypeIndex(query.Subtype);
            if (index == -1) return "[Bad Subtype]";

            var pool =
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