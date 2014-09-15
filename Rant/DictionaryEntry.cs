using System.Collections.Generic;

namespace Rant
{
    /// <summary>
    /// Stores information about a dictionary entry.
    /// </summary>
    public sealed class DictionaryEntry
    {
        private string[] _values;
        private HashSet<string> _classes;
        private int _weight;

        /// <summary>
        /// Creates a new Word object from the specified data.
        /// </summary>
        /// <param name="entries">The values of the entry.</param>
        /// <param name="classes">The classes associated with the entry.</param>
        /// <param name="weight">The weight of the entry.</param>
        public DictionaryEntry(string[] entries, IEnumerable<string> classes, int weight = 1)
        {
            _values = entries ?? new string[0];
            _classes = new HashSet<string>(classes);
            _weight = weight;
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
    }
}