using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Stores information about a dictionary entry.
    /// </summary>
    public sealed class RantDictionaryEntry
    {
        private RantDictionaryTerm[] _terms;
        private readonly HashSet<string> _classes;
        private readonly HashSet<string> _optionalClasses;
        private int _weight;
        private readonly bool _nsfw;

        /// <summary>
        /// Creates a new Word object from the specified data.
        /// </summary>
        /// <param name="terms">The terms in the entry.</param>
        /// <param name="classes">The classes associated with the entry.</param>
        /// <param name="weight">The weight of the entry.</param>
        /// <param name="nsfw">Specified if the entry should be marked with a NSFW flag.</param>
        public RantDictionaryEntry(string[] terms, IEnumerable<string> classes, bool nsfw = false, int weight = 1)
        {
            _terms = terms.Select(s => new RantDictionaryTerm(s)).ToArray();
            _classes = new HashSet<string>(classes);
            _optionalClasses = new HashSet<string>();
            _weight = weight;
            _nsfw = nsfw;
        }

        /// <summary>
        /// Creates a new Word object from the specified data.
        /// </summary>
        /// <param name="terms">The terms in the entry.</param>
        /// <param name="classes">The classes associated with the entry.</param>
        /// <param name="weight">The weight of the entry.</param>
        /// <param name="nsfw">Specified if the entry should be marked with a NSFW flag.</param>
        public RantDictionaryEntry(RantDictionaryTerm[] terms, IEnumerable<string> classes, bool nsfw = false, int weight = 1)
        {
            _terms = terms;
            _classes = new HashSet<string>();
            _optionalClasses = new HashSet<string>();
            foreach(var c in classes)
            {
                VocabUtils.SortClass(c, _classes, _optionalClasses);
            }
            _weight = weight;
            _nsfw = nsfw;
        }

        /// <summary>
        /// Gets the value for the specified term index in the entry. If the index is out of range, [Missing Term] will be returned.
        /// </summary>
        /// <param name="index">The index of the term whose value to request.</param>
        /// <returns></returns>
        public string this[int index] => (index < 0 || index >= _terms.Length) ? "[Missing Term]" : _terms[index].Value;

        /// <summary>
        /// The terms in the entry.
        /// </summary>
        public RantDictionaryTerm[] Terms
        {
            get { return _terms; }
            set { _terms = value ?? new RantDictionaryTerm[0]; }
        }

        /// <summary>
        /// The classes associated with the entry.
        /// </summary>
        public HashSet<string> Classes => _classes;

        /// <summary>
        /// The optional classes associated with the entry.
        /// </summary>
        public HashSet<string> OptionalClasses => _optionalClasses;

        /// <summary>
        /// The weight of the entry.
        /// </summary>
        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        /// <summary>
        /// Indicates if the entry is marked as "Not Safe For Work."
        /// </summary>
        public bool NSFW => _nsfw;
    }
}