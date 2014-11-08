using System.Collections.Generic;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Stores information about a dictionary entry.
    /// </summary>
    public sealed class RantDictionaryEntry
    {
        private string[] _values;
        private string[] _ipa;
        private HashSet<string> _classes;
        private int _weight;
        private bool _nsfw;

        /// <summary>
        /// Creates a new Word object from the specified data.
        /// </summary>
        /// <param name="entries">The values of the entry.</param>
        /// <param name="classes">The classes associated with the entry.</param>
        /// <param name="weight">The weight of the entry.</param>
        public RantDictionaryEntry(string[] entries, IEnumerable<string> classes, bool nsfw = false, int weight = 1)
        {
            _values = entries ?? new string[0];
            _classes = new HashSet<string>(classes);
            _weight = weight;
            _nsfw = nsfw;
            _ipa = new string[entries.Length];
        }

        /// <summary>
        /// Creates a new Word object from the specified data.
        /// </summary>
        /// <param name="entries">The values of the entry.</param>
        /// <param name="classes">The classes associated with the entry.</param>
        /// <param name="weight">The weight of the entry.</param>
        public RantDictionaryEntry(string[] entries, IEnumerable<string> classes, string[] ipa, bool nsfw = false, int weight = 1)
        {
            _values = entries ?? new string[0];
            _classes = new HashSet<string>(classes);
            _weight = weight;
            _nsfw = nsfw;
            _ipa = ipa.Length == entries.Length ? ipa : new string[entries.Length];
        }

        /// <summary>
        /// The values of the entry.
        /// </summary>
        public string[] Values
        {
            get { return _values; }
            set { _values = value ?? new string[0]; }
        }

        /// <summary>
        /// The classes associated with the entry.
        /// </summary>
        public HashSet<string> Classes
        {
            get { return _classes; }
            set { _classes = value ?? new HashSet<string>(); }
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
        public bool NSFW
        {
            get { return _nsfw; }
        }

        /// <summary>
        /// The IPA pronunciation strings for the values.
        /// </summary>
        public string[] IPA
        {
            get { return _ipa; }
            set { _ipa = value; }
        }
    }
}