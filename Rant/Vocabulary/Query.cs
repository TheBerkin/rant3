using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Represents a set of search criteria for a Rant dictionary.
    /// </summary>
    public sealed class Query
    {
        /// <summary>
        /// The carrier for the query.
        /// </summary>
        public Carrier Carrier { get; set; }

	    /// <summary>
        /// The name of the dictionary to search.
        /// </summary>
        public string Name { get; set; }

	    /// <summary>
        /// The subtype of the dictionary entry to use.
        /// </summary>
        public string Subtype { get; set; }

	    /// <summary>
        /// Specifies exclusivity of class filters.
        /// </summary>
        public bool Exclusive { get; set; }

	    /// <summary>
        /// The syllable range predicate. Set to null for no syllable count filtering.
        /// </summary>
        public Range SyllablePredicate { get; set; }

	    /// <summary>
        /// The class filters to search by.
        /// </summary>
        public IEnumerable<_<bool, string>[]> ClassFilters { get; set; }

	    /// <summary>
        /// The regex filters to search by.
        /// </summary>
        public IEnumerable<_<bool, Regex>> RegexFilters { get; set; }

		/// <summary>
		/// The origin stringe of the query.
		/// </summary>
		internal Rant.Stringes.Stringe OriginStringe { get; set; }
	}
}