using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.Compiler.Syntax;

namespace Rant.Vocabulary.Querying
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
		/// Specifies exclusivity of the class filter.
		/// </summary>
		public bool Exclusive { get; set; }

		/// <summary>
		/// The syllable range predicate. Set to null for no syllable count filtering.
		/// </summary>
		public Range SyllablePredicate { get; set; }

		/// <summary>
		/// The class filter to search by.
		/// </summary>
		public ClassFilter ClassFilter { get; set; }

		/// <summary>
		/// The regex filters to search by.
		/// </summary>
		public List<_<bool, Regex>> RegexFilters { get; set; }

		/// <summary>
		/// Complement for phrasal verbs. Not yet available in public API.
		/// </summary>
		internal RST Complement { get; set; }
	}
}