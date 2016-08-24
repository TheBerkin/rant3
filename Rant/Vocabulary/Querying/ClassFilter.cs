using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary.Querying
{
	/// <summary>
	/// Defines a set of class filtering rules for a query.
	/// </summary>
	public sealed class ClassFilter
	{
		private readonly List<ClassFilterRule[]> _items = new List<ClassFilterRule[]>();

		/// <summary>
		/// Gets a boolean value indicating whether there are any rules added to the current ClassFilter instance.
		/// </summary>
		public bool IsEmpty => _items.Count == 0;

		/// <summary>
		/// Adds a single-class rule to the filter.
		/// </summary>
		/// <param name="item"></param>
		public void AddRule(ClassFilterRule item)
		{
			_items.Add(new[] { item });
		}

		/// <summary>
		/// Adds a rule set that must satisfy one of the specified rules.
		/// </summary>
		/// <param name="items">The items to include in the rule switch.</param>
		public void AddRuleSwitch(params ClassFilterRule[] items)
		{
			_items.Add(items);
		}

		/// <summary>
		/// Determines if the specified dictionary entry passes the filter.
		/// </summary>
		/// <param name="entry">The entry to test.</param>
		/// <param name="exclusive">Specifies whether the search is exclusive.</param>
		/// <returns></returns>
		public bool Test(RantDictionaryEntry entry, bool exclusive = false)
		{
			return exclusive
				? _items.Any() == entry.GetClasses().Any()
				  && entry.GetClasses().All(c => _items.Any(item => item.Any(rule => rule.ShouldMatch && rule.Class == c)))
				: !_items.Any() || _items.All(set => set.Any(rule => entry.ContainsClass(rule.Class) == rule.ShouldMatch));
		}

		/// <summary>
		/// Returns a boolean value indicating whether the specified class is explicitly allowed by the current ClassFilter.
		/// </summary>
		/// <param name="className">The class to test.</param>
		/// <returns></returns>
		public bool AllowsClass(string className) =>
			_items.Any(item => item.Any(rule => (rule.Class == className) && rule.ShouldMatch));
	}
}