using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Stores information about a dictionary entry.
	/// </summary>
	public sealed class RantDictionaryEntry
	{
		private const int INITIAL_METADATA_CAPACITY = 4;
		private readonly HashSet<string> _classes;
		private readonly Lazy<Dictionary<string, object>> _metadata = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>(INITIAL_METADATA_CAPACITY));
		private readonly HashSet<string> _optionalClasses;
		private readonly RantDictionaryTerm[] _terms;

		/// <summary>
		/// Creates a new instance of the <see cref="RantDictionaryEntry" /> object from the specified term array.
		/// </summary>
		/// <param name="terms">The terms in the entry.</param>
		public RantDictionaryEntry(RantDictionaryTerm[] terms)
		{
			if (terms == null) throw new ArgumentNullException(nameof(terms));
			_terms = terms.ToArray();
			if (_terms.Length == 0) throw new ArgumentException("Term list is empty.");
			TermCount = _terms.Length;
			_classes = new HashSet<string>();
			_optionalClasses = new HashSet<string>();
			Weight = 1;
		}

		/// <summary>
		/// Creates a new <see cref="RantDictionaryEntry" /> object from the specified term array, classes, and weight.
		/// </summary>
		/// <param name="terms">The terms in the entry.</param>
		/// <param name="classes">The classes associated with the entry.</param>
		/// <param name="weight">The weight of the entry.</param>
		public RantDictionaryEntry(string[] terms, IEnumerable<string> classes, int weight = 1)
			: this(terms.Select(s => new RantDictionaryTerm(s)), classes, weight)
		{
		}

		/// <summary>
		/// Creates a new <see cref="RantDictionaryEntry" /> object from the specified term collection, classes, and weight.
		/// </summary>
		/// <param name="terms">The terms in the entry.</param>
		/// <param name="classes">The classes associated with the entry.</param>
		/// <param name="weight">The weight of the entry.</param>
		public RantDictionaryEntry(IEnumerable<RantDictionaryTerm> terms, IEnumerable<string> classes, int weight = 1)
		{
			if (terms == null) throw new ArgumentNullException(nameof(terms));
			_terms = terms.ToArray();
			if (_terms.Length == 0) throw new ArgumentException("Term list is empty.");
			TermCount = _terms.Length;
			_classes = new HashSet<string>();
			_optionalClasses = new HashSet<string>();
			foreach (string c in classes)
			{
				if (c.EndsWith("?"))
				{
					string trimmed = string.Intern(c.Substring(0, c.Length - 1));
					_optionalClasses.Add(trimmed);
					_classes.Add(trimmed);
				}
				else
				{
					_classes.Add(string.Intern(c));
				}
			}
			Weight = weight;
		}

		/// <summary>
		/// Gets the number of terms stored in the current entry.
		/// </summary>
		public int TermCount { get; }

		/// <summary>
		/// Gets or sets the term at the specified index.
		/// </summary>
		/// <param name="index">The index of the term to access.</param>
		/// <returns></returns>
		public RantDictionaryTerm this[int index]
		{
			get { return (index < 0 || index >= _terms.Length) ? null : _terms[index]; }
			set
			{
				if (index < 0 || index <= _terms.Length)
					throw new IndexOutOfRangeException("Index was outside of the bounds of the entry's term list.");

				_terms[index] = value;
			}
		}

		/// <summary>
		/// Gets the weight value of the entry.
		/// </summary>
		public int Weight { get; set; }

		/// <summary>
		/// Enumerates the terms stored in the current entry.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RantDictionaryTerm> GetTerms() => _terms.AsEnumerable();

		/// <summary>
		/// Returns a collection of classes assigned to the current entry.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetClasses()
		{
			foreach (string className in _classes) yield return className;
		}

		/// <summary>
		/// Returns a collection of the optional classes assigned to the current entry.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetOptionalClasses()
		{
			foreach (string className in _optionalClasses) yield return className;
		}

		/// <summary>
		/// Adds the specified class to the current entry.
		/// </summary>
		/// <param name="className">The name of the class.</param>
		/// <param name="optional">Specifies whether the class is optional in carrier associations.</param>
		public void AddClass(string className, bool optional = false)
		{
			if (className.Trim().EndsWith("?"))
			{
				optional = true;
				className = string.Intern(className.Trim().TrimEnd('?'));
			}
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
		/// Sets a metadata value under the specified key in the entry.
		/// </summary>
		/// <param name="key">The key to store the data under.</param>
		/// <param name="value">The value to store.</param>
		public void SetMetadata(string key, object value)
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			_metadata.Value[key] = value;
		}

		/// <summary>
		/// Removes the metadata with the specified key from the entry.
		/// </summary>
		/// <param name="key">The key of the metadata entry to remove.</param>
		/// <returns></returns>
		public bool RemoveMetadata(string key)
		{
			return key != null && _metadata.IsValueCreated && _metadata.Value.Remove(key);
		}

		/// <summary>
		/// Enumerates all the metadata keys contained in the entry.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetMetadataKeys()
		{
			if (!_metadata.IsValueCreated) yield break;
			foreach (string key in _metadata.Value.Keys) yield return key;
		}

		/// <summary>
		/// Locates and returns the metadata value associated with the specified key. Returns Null if not found.
		/// </summary>
		/// <param name="key">The key of the metadata to retrieve.</param>
		/// <returns></returns>
		public object GetMetadata(string key)
		{
			if (!_metadata.IsValueCreated) return null;
			object result;
			return !_metadata.Value.TryGetValue(key, out result) ? null : result;
		}

		/// <summary>
		/// Determines if the entry contains metadata attached to the specified key.
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns></returns>
		public bool ContainsMetadataKey(string key)
		{
			if (key == null || !_metadata.IsValueCreated) return false;
			return _metadata.Value.ContainsKey(key);
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
		/// Returns a string representation of the current <see cref="RantDictionaryEntry" /> instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => _terms.Any() ? _terms[0].Value : "???";
	}
}