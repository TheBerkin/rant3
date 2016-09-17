using System.Collections.Generic;
using System.Linq;

using Rant.Core.IO;

namespace Rant.Vocabulary.Querying
{
	/// <summary>
	/// Defines a set of class filtering rules for a query.
	/// </summary>
	internal sealed class ClassFilter : Filter
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

		public override bool Test(RantDictionary dictionary, RantDictionaryTable table, RantDictionaryEntry entry, int termIndex, Query query)
		{
			bool match = query.Exclusive
				? _items.Any() == entry.GetClasses().Any()
				  && entry.GetClasses().All(c => _items.Any(item => item.Any(rule => rule.ShouldMatch && rule.Class == c)))
				: !_items.Any() || _items.All(set => set.Any(rule => entry.ContainsClass(rule.Class) == rule.ShouldMatch));

			// Enumerate hidden classes that aren't manually exposed or explicitly allowed by the filter
			var hidden = table.HiddenClasses.Where(c => !AllowsClass(c)).Except(dictionary.IncludedHiddenClasses);
			return match && !hidden.Any(entry.ContainsClass);
		}

		/// <summary>
		/// Returns a boolean value indicating whether the specified class is explicitly allowed by the current ClassFilter.
		/// </summary>
		/// <param name="className">The class to test.</param>
		/// <returns></returns>
		public bool AllowsClass(string className) =>
			_items.Any(item => item.Any(rule => (rule.Class == className) && rule.ShouldMatch));

		public override void Serialize(EasyWriter output)
		{
			output.Write(FILTER_CLASS);
			output.Write(_items.Count);
			foreach (var filter in _items)
			{
				output.Write(filter.Length);
				foreach (var rule in filter)
				{
					output.Write(rule.ShouldMatch);
					output.Write(rule.Class);
				}
			}
		}

		public override void Deserialize(EasyReader input)
		{
			int count = input.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				int swLength = input.ReadInt32();
				var sw = new ClassFilterRule[swLength];
				for (int j = 0; j < swLength; j++)
				{
					bool shouldMatch = input.ReadBoolean();
					string clName = input.ReadString();
					sw[j] = new ClassFilterRule(clName, shouldMatch);
				}
				_items.Add(sw);
			}
		}
	}
}