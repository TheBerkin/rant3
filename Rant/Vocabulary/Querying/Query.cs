using System.Collections.Generic;

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
		/// The name of the table to search.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The subtype of the dictionary term to use.
		/// </summary>
		public string Subtype { get; set; }

		/// <summary>
		/// The plural subtype to use.
		/// </summary>
		public string PluralSubtype { get; set; }

		/// <summary>
		/// Specifies exclusivity of the class filter.
		/// </summary>
		public bool Exclusive { get; set; }

		/// <summary>
		/// Complement for phrasal verbs. Not yet available in public API.
		/// </summary>
		internal RST Complement { get; set; }

		public int FilterCount => _filters.Count;

		/// <summary>
		/// Returns whether the query is a "bare query" - should only return the table itself.
		/// </summary>
		public bool BareQuery => _filters.Count == 0 && !Exclusive && !HasCarrier;

		/// <summary>
		/// Returns whether the query has a carrier.
		/// </summary>
		public bool HasCarrier => Carrier != null && Carrier.GetTotalCount() != 0;

		public void AddFilter(Filter filter) => _filters.Add(filter);

		public IEnumerable<Filter> GetFilters()
		{
			foreach (var filter in _filters) yield return filter;
		}
	}
}