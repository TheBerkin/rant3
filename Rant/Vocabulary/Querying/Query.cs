using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.Compiler.Syntax;

namespace Rant.Vocabulary.Querying
{
	/// <summary>
	/// Represents a set of search criteria for a Rant dictionary.
	/// </summary>
	internal sealed class Query
	{
		private readonly HashSet<Filter> _filters = new HashSet<Filter>();

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
		/// Complement for phrasal verbs. Not yet available in public API.
		/// </summary>
		internal RST Complement { get; set; }

		public void AddFilter(Filter filter) => _filters.Add(filter);

		public IEnumerable<Filter> GetFilters()
		{
			foreach (var filter in _filters) yield return filter;
		} 

		public int FilterCount => _filters.Count;
	}
}