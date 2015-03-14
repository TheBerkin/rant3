using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Rant.Engine;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Represents a set of search criteria for a Rant dictionary.
    /// </summary>
    public sealed class Query
    {
        private string _name;
        private string _subtype;
        private Carrier _carrier;
        private SyllablePredicateFunc _syllableRange;
        private bool _exclusive;
        private readonly IEnumerable<_<bool, string>[]> _classFilters;
        private readonly IEnumerable<_<bool, Regex>> _regexFilters;

        /// <summary>
        /// Creates a new Query object with the specified arguments.
        /// </summary>
        /// <param name="name">The name of the dictionary to search.</param>
        /// <param name="subtype">The subtype of the dictionary entry to use.</param>
        /// <param name="carrier">The carrier for the query.</param>
        /// <param name="exclusive">Specifies exclusivity of class filters.</param>
        /// <param name="classFilters">The class filters to search by.</param>
        /// <param name="regexFilters">The regex filters to search by.</param>
        /// <param name="syllableRange">The syllable range to constrain results to.</param>
        public Query(string name, string subtype, Carrier carrier, bool exclusive, IEnumerable<_<bool, string>[]> classFilters,
            IEnumerable<_<bool, Regex>> regexFilters, SyllablePredicateFunc syllableRange)
        {
            _name = name;
            _subtype = subtype;
            _exclusive = exclusive;
            _carrier = carrier;
            _classFilters = (classFilters ?? Enumerable.Empty<_<bool, string>[]>()).AsEnumerable();
            _regexFilters = (regexFilters ?? Enumerable.Empty<_<bool, Regex>>()).AsEnumerable();
            _syllableRange = syllableRange;
        }

        /// <summary>
        /// The carrier for the query.
        /// </summary>
        public Carrier Carrier
        {
            get { return _carrier; }
            set { _carrier = value; }
        }

        /// <summary>
        /// The name of the dictionary to search.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The subtype of the dictionary entry to use.
        /// </summary>
        public string Subtype
        {
            get { return _subtype; }
            set { _subtype = value; }
        }

        /// <summary>
        /// Specifies exclusivity of class filters.
        /// </summary>
        public bool Exclusive
        {
            get { return _exclusive; }
            set { _exclusive = value; }
        }

        /// <summary>
        /// The syllable range predicate. Set to null for no syllable count filtering.
        /// </summary>
        public SyllablePredicateFunc SyllablePredicate
        {
            get { return _syllableRange; }
            set { _syllableRange = value; }
        }

        /// <summary>
        /// The class filters to search by.
        /// </summary>
        public IEnumerable<_<bool, string>[]> ClassFilters
        {
            get { return _classFilters; }
        }

        /// <summary>
        /// The regex filters to search by.
        /// </summary>
        public IEnumerable<_<bool, Regex>> RegexFilters
        {
            get { return _regexFilters; }
        }
    }
}