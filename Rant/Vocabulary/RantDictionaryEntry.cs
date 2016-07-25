using System.Collections.Generic;
using System.Linq;

using Rant.Vocabulary.Utilities;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Stores information about a dictionary entry.
	/// </summary>
	public sealed class RantDictionaryEntry
	{
		private RantDictionaryTerm[] _terms;
		private readonly HashSet<string> _classes;
		private readonly HashSet<string> _optionalClasses;

		/// <summary>
		/// Creates a new RantDictionaryEntry object from the specified data.
		/// </summary>
		/// <param name="terms">The terms in the entry.</param>
		/// <param name="classes">The classes associated with the entry.</param>
		/// <param name="weight">The weight of the entry.</param>
		public RantDictionaryEntry(string[] terms, IEnumerable<string> classes, int weight = 1)
			: this(terms.Select(s => new RantDictionaryTerm(s)).ToArray(), classes, weight)
		{
		}

		/// <summary>
		/// Creates a new RantDictionaryEntry object from the specified data.
		/// </summary>
		/// <param name="terms">The terms in the entry.</param>
		/// <param name="classes">The classes associated with the entry.</param>
		/// <param name="weight">The weight of the entry.</param>
		public RantDictionaryEntry(RantDictionaryTerm[] terms, IEnumerable<string> classes, int weight = 1)
		{
			_terms = terms;
			_classes = new HashSet<string>();
			_optionalClasses = new HashSet<string>();
			foreach (var c in classes)
			{
				if (c.EndsWith("?"))
				{
					var trimmed = VocabUtils.GetString(c.Substring(0, c.Length - 1));
					_optionalClasses.Add(trimmed);
					_classes.Add(trimmed);
				}
				else
				{
					_classes.Add(VocabUtils.GetString(c));
				}
			}
			Weight = weight;
		}

		/// <summary>
		/// Gets the value for the specified term index in the entry. If the index is out of range, [Missing Term] will be returned.
		/// </summary>
		/// <param name="index">The index of the term whose value to request.</param>
		/// <returns></returns>
		public string this[int index] => (index < 0 || index >= _terms.Length) ? "[Missing Term]" : _terms[index].Value;

		/// <summary>
		/// The terms in the entry.
		/// </summary>
		public RantDictionaryTerm[] Terms
		{
			get { return _terms; }
			set { _terms = value ?? new RantDictionaryTerm[0]; }
		}

		/// <summary>
		/// Returns a collection of classes assigned to the current entry.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetClasses()
		{
			foreach (var className in _classes) yield return className;
		}

		/// <summary>
		/// Returns a collection of the optional classes assigned to the current entry.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetOptionalClasses()
		{
			foreach (var className in _optionalClasses) yield return className;
		}

		/// <summary>
		/// Adds the specified class to the current entry.
		/// </summary>
		/// <param name="className">The name of the class.</param>
		/// <param name="optional">Specifies whether the class is optional in carrier associations.</param>
		public void AddClass(string className, bool optional = false)
		{
			_classes.Add(className);
			if (optional) _optionalClasses.Add(className);
		}

		/// <summary>
		/// Removes the class with the specified name from the current entry.
		/// </summary>
		/// <param name="className">The name of the class to remove.</param>
		public void RemoveClass(string className)
		{
			_classes.Remove(className);
			_optionalClasses.Remove(className);
		}

		/// <summary>
		/// Returns a boolean valie indicating whether the current entry contains the specified class.
		/// </summary>
		/// <param name="className">The class to search for.</param>
		/// <returns></returns>
		public bool ContainsClass(string className) => _classes.Contains(className);

		/// <summary>
		/// Returns a collection of required (non-optional) classes assigned to the current entry.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetRequiredClasses() => _classes.Except(_optionalClasses);

		/// <summary>
		/// Gets the weight value of the entry.
		/// </summary>
		public int Weight { get; set; }

		/// <summary>
		/// Returns a string representation of the current RantDictionaryEntry instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => _terms.Any() ? _terms[0].Value : "???";
	}
}