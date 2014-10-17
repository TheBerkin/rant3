using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant
{
    /// <summary>
    /// Stores a Rant dictionary.
    /// </summary>
    public sealed partial class Dictionary
    {
        private readonly string _name;
        private readonly string[] _subtypes;
        private readonly DictionaryEntry[] _words;

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
                throw new FormatException("Invalid dictionary name: '" + name + "'");
            }

            if (!subtypes.All(Util.ValidateName))
            {
                throw new FormatException("Invalid subtype name(s): " + 
                    subtypes.Where(s => !Util.ValidateName(s))
                    .Select(s => String.Concat("'", s, "'"))
                    .Aggregate((c,n) => c + ", " + n));
            }

            _subtypes = subtypes;
            _name = name;
            _words = words.ToArray();
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

        internal string Query(RNG rng, Query query)
        {
            var index = String.IsNullOrEmpty(query.Subtype) ? 0 : GetSubtypeIndex(query.Subtype);
            if (index == -1)
            {
                return "BAD-SUBTYPE";
            }

            IEnumerable<DictionaryEntry> pool = _words;

            pool = query.Exclusive
                ? pool.Where(e => e.Classes.Any() && e.Classes.All(
                    c => query.ClassFilters.Any(
                        set => set.Any(t => t.Item2 == c))))
                : pool.Where(e => query.ClassFilters.All(
                    set => set.Any(
                        t => t.Item1 == (e.Classes.Contains(t.Item2)))));

            pool = query.RegexFilters.Aggregate(pool, (current, regex) => current.Where(e => regex.Item1 == regex.Item2.IsMatch(e.Values[index])));

            if (!pool.Any())
            {
                return "NOT-FOUND";
            }

            int number = String.IsNullOrEmpty(query.Carrier) ? rng.Next(pool.Sum(e => e.Weight)) + 1
                : rng.PeekAt(query.Carrier.Hash(), pool.Sum(e => e.Weight));

            foreach (var e in pool)
            {
                if (number <= e.Weight) return e.Values[index];
                number -= e.Weight;
            }

            return "NOT-FOUND";
        }
    }
}