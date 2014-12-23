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
        /// Creates a new RantDictionaryEntry object from the specified data.
        /// </summary>
        /// <param name="terms">The terms in the entry.</param>
        /// <param name="classes">The classes associated with the entry.</param>
        /// <param name="weight">The weight of the entry.</param>
        /// <param name="nsfw">Specified if the entry should be marked with a NSFW flag.</param>
        public RantDictionaryEntry(string[] terms, IEnumerable<string> classes, bool nsfw = false, int weight = 1)
            : this(terms.Select(s => new RantDictionaryTerm(s)).ToArray(), classes, nsfw, weight)
        {
        }

        /// <summary>
        /// Creates a new RantDictionaryEntry object from the specified data.
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
                if (c.EndsWith("?"))
                {
                    var trimmed = VocabUtils.GetString(c.Substring(0, c.Length - 1));
                    _optionalClasses.Add(trimmed);
                    _classes.Add(trimmed);
                }
                else
                {
                    _classes.Add(VocabUtils.GetString(c));
                }
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

        public IEnumerable<string> GetClasses()
        {
            foreach (var className in _classes) yield return className;
        }

        public IEnumerable<string> GetOptionalClasses()
        {
            foreach (var className in _optionalClasses) yield return className;
        }

        public void AddClass(string className, bool optional = false)
        {
            _classes.Add(className);
            if (optional) _optionalClasses.Add(className);
        }

        public void RemoveClass(string className)
        {
            _classes.Remove(className);
            _optionalClasses.Remove(className);
        }

        public bool ContainsClass(string className) => _classes.Contains(className);

        public IEnumerable<string> GetRequiredClasses()
        {
            foreach(var c in _classes.Except(_optionalClasses))
            {
                yield return c;
            }
        }

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