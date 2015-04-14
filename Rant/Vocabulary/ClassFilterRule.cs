using System;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Defines a query filter for a single dictionary entry class.
	/// </summary>
	public sealed class ClassFilterRule
	{
		/// <summary>
		/// Determines whether the filter item expects a positive or negative match for the class.
		/// </summary>
		public bool ShouldMatch { get; set; } = true;

		/// <summary>
		/// The name of the class to search for.
		/// </summary>
		public string Class { get; set; }

		/// <summary>
		/// Initializes a new ClassFilterRule that checks for a positive match to the specified class.
		/// </summary>
		/// <param name="className">The name of the class to search for.</param>
		public ClassFilterRule(string className)
		{
			Class = className;
		}

		/// <summary>
		/// Initializes a new ClassFilterRule that checks for a positive or negative match to the specified class.
		/// </summary>
		/// <param name="className">The name of the class to search for.</param>
		/// <param name="shouldMatch">Determines whether the filter item expects a positive or negative match for the class.</param>
		public ClassFilterRule(string className, bool shouldMatch)
		{
			Class = className;
			ShouldMatch = shouldMatch;
		}
	}
}