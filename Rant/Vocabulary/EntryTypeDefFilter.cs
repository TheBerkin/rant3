using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant.Vocabulary
{
	internal class EntryTypeDefFilter
	{
		private readonly Regex _filterRegex = new Regex(@"!?\w+");
		private readonly _<string, bool>[] _filterParts;

		public EntryTypeDefFilter(string filter)
		{
			if (filter.Trim() == "*") return;
			_filterParts = filter
				.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Where(s => _filterRegex.IsMatch(s))
				.Select(s => _.Create(s.TrimStart('!'), s.StartsWith("!")))
				.ToArray();
		}

		private bool DoTest(RantDictionaryEntry entry)
		{
			return _filterParts == null || _filterParts.All(f => entry.ContainsClass(f.Item1) == f.Item2);
		}

		/// <summary>
		/// Determines whether a type should apply to the specifed entry according to the specified filter.
		/// </summary>
		/// <param name="filter">The filter to test with.</param>
		/// <param name="entry">The entry to test.</param>
		/// <returns></returns>
		public static bool Test(EntryTypeDefFilter filter, RantDictionaryEntry entry) => filter?.DoTest(entry) ?? false;
	}
}