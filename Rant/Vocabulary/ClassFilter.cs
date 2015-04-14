using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Defines a set of class filtering rules for a query.
	/// </summary>
	public sealed class ClassFilter
	{
		private readonly List<ClassFilterRule[]> _items = new List<ClassFilterRule[]>();

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
	}
}