using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Core.Utilities;
using Rant.Vocabulary.Querying;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Represents a named collection of dictionary entries.
	/// </summary>
	public sealed partial class RantDictionaryTable
	{
		internal const string MissingTerm = "[?]";
		private readonly HashSet<RantDictionaryEntry> _entriesHash = new HashSet<RantDictionaryEntry>();
		private readonly List<RantDictionaryEntry> _entriesList = new List<RantDictionaryEntry>(); // TODO: Use for indexing / weighted selection
		private readonly HashSet<string> _hidden = new HashSet<string>();
		private readonly int _termsPerEntry;
		private readonly Dictionary<string, int> _subtypes = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
		private readonly Dictionary<int, HashSet<string>> _subtypeIndexMap = new Dictionary<int, HashSet<string>>(); 

		/// <summary>
		/// Initializes a new instance of the RantDictionaryTable class with the specified name and term count.
		/// </summary>
		/// <param name="name">The name of the table.</param>
		/// <param name="termsPerEntry">The number of terms to store in each entry.</param>
		public RantDictionaryTable(string name, int termsPerEntry)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (termsPerEntry <= 0) throw new ArgumentException("Terms per entry must be greater than zero.");
			if (!Util.ValidateName(name)) throw new ArgumentException($"Invalid table name: '{name}'");
			_termsPerEntry = termsPerEntry;
			Name = name;
		}

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the language code associated with the table (not yet used).
		/// </summary>
		public string Language { get; } = "en-US";

		/// <summary>
		/// Gets the hidden classes of the table.
		/// </summary>
		public IEnumerable<string> HiddenClasses => _hidden.AsEnumerable();

		/// <summary>
		/// Gets the number of entries stored in the table.
		/// </summary>
		public int EntryCount => _entriesHash.Count;

		/// <summary>
		/// Enumerates the entries stored in the table.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RantDictionaryEntry> GetEntries()
		{
			foreach (var entry in _entriesHash) yield return entry;
		}

		/// <summary>
		/// Gets the number of terms required for entries contained in the current table.
		/// </summary>
		public int TermsPerEntry => _termsPerEntry;

		/// <summary>
		/// Enumerates the subtypes contained in the current table.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetSubtypes() => _subtypes.Keys;

		/// <summary>
		/// Determines whether the specified class is hidden by the table.
		/// </summary>
		/// <param name="className">The name of the class to check.</param>
		/// <returns></returns>
		public bool IsClassHidden(string className)
		{
			if (className == null) throw new ArgumentNullException(nameof(className));
			return _hidden.Contains(className);
		}

		/// <summary>
		/// Hides the specified class.
		/// </summary>
		/// <param name="className">The name of the class to hide.</param>
		/// <returns></returns>
		public bool HideClass(string className) => Util.ValidateName(className) && _hidden.Add(className);

		/// <summary>
		/// Unhides the specified class.
		/// </summary>
		/// <param name="className">The name of the class to unhide.</param>
		/// <returns></returns>
		public bool UnhideClass(string className) => className != null && _hidden.Remove(className);

		/// <summary>
		/// Adds the specified entry to the table.
		/// </summary>
		/// <param name="entry">The entry to add to the table.</param>
		/// <returns>True if successfully added; otherwise, False.</returns>
		public bool AddEntry(RantDictionaryEntry entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			if (entry.TermCount != _termsPerEntry) return false;
			if (!_entriesHash.Add(entry)) return false;
			_entriesList.Add(entry);
			return true;
		}

		/// <summary>
		/// Removes the specified entry from the table.
		/// </summary>
		/// <param name="entry">The entry to remove from the table.</param>
		/// <returns>True if successfully removed; otherwise, False.</returns>
		public bool RemoveEntry(RantDictionaryEntry entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			if (!_entriesHash.Remove(entry)) return false;
			_entriesList.Remove(entry);
			return true;
		}

		/// <summary>
		/// Checks if the table contains the specified entry.
		/// </summary>
		/// <param name="entry">The entry to search for.</param>
		/// <returns>True if found, False if not.</returns>
		public bool ContainsEntry(RantDictionaryEntry entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			return _entriesHash.Contains(entry);
		}

		/// <summary>
		/// Searches entries in the current table and enumerates every single distinct class found.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetClasses()
		{
			var lstClasses = new HashSet<string>();

			foreach (string c in _entriesHash.SelectMany(e => e.GetClasses()))
			{
				if (lstClasses.Add(c)) yield return c;
			}
		}

		/// <summary>
		/// Adds a subtype of the specified name to the table.
		/// If a subtype with the name already exists, it will be overwritten.
		/// Subtypes are case insensitive.
		/// If the name is not a valid identifier string, it will not be accepted.
		/// </summary>
		/// <param name="subtypeName">The name of the subtype to add.</param>
		/// <param name="index">The term index to associate with the name.</param>
		/// <returns>FALSE if the name was not a valid identifier or the index was out of range. TRUE if the operation was successful.</returns>
		public bool AddSubtype(string subtypeName, int index)
		{
			if (index < 0 || index >= _termsPerEntry) return false;
			if (subtypeName == null) throw new ArgumentNullException(nameof(subtypeName));
			if (!Util.ValidateName(subtypeName)) return false;
			_subtypes[subtypeName] = index;
			HashSet<string> subs;
			if (!_subtypeIndexMap.TryGetValue(index, out subs))
			{
				_subtypeIndexMap[index] = subs = new HashSet<string>();
			}
			subs.Add(subtypeName);
			return true;
		}

		/// <summary>
		/// Removes the specified subtype from the table, if it exists.
		/// Subtypes are case insensitive.
		/// </summary>
		/// <param name="subtypeName">The name of the subtype to remove.</param>
		/// <returns>TRUE if the subtype was found and removed. FALSE if the subtype was not found.</returns>
		public bool RemoveSubtype(string subtypeName)
		{
			if (Util.IsNullOrWhiteSpace(subtypeName)) return false;
			if (!_subtypes.ContainsKey(subtypeName)) return false;
			HashSet<string> subs;
			if (_subtypeIndexMap.TryGetValue(_subtypes[subtypeName], out subs))
			{
				return subs.Remove(subtypeName) && _subtypes.Remove(subtypeName);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Retrieves the term index assigned to the specified subtype.
		/// If the subtype is not found, the method will return -1.
		/// If the subtype is a null, whitespace, or an empty string, the method will return 0.
		/// </summary>
		/// <param name="subtype">The subtype to look up.</param>
		/// <returns></returns>
		public int GetSubtypeIndex(string subtype)
		{	
			if (Util.IsNullOrWhiteSpace(subtype)) return 0;
			if (!Util.ValidateName(subtype)) return -1;
			int index;
			if (int.TryParse(subtype, out index) && index >= 0) return index;
			return _subtypes.TryGetValue(subtype, out index) ? index : -1;
		}

		/// <summary>
		/// Enumerates the subtypes associated with the specified term index.
		/// </summary>
		/// <param name="index">The index to get subtypes for.</param>
		/// <returns></returns>
		public IEnumerable<string> GetSubtypesForIndex(int index)
		{
			if (index < 0 || index >= _termsPerEntry) yield break;
			HashSet<string> subs;
			if (!_subtypeIndexMap.TryGetValue(index, out subs)) yield break;
			foreach (var sub in subs) yield return sub;
		}  

		/// <summary>
		/// Adds another table's entries to the current table, given that they share the same name and term count.
		/// </summary>
		/// <param name="other">The table whose entries will be added to the current instance.</param>
		/// <returns>True if merge succeeded; otherwise, False.</returns>
		public bool Merge(RantDictionaryTable other)
		{
			if (other.Name != Name || other == this) return false;
			if (other._termsPerEntry != _termsPerEntry) return false;
			_entriesHash.AddRange(other._entriesHash);
			_entriesList.AddRange(other._entriesHash);
			return true;
		}

		internal RantDictionaryTerm Query(RantDictionary dictionary, RNG rng, Query query, CarrierState syncState)
		{
			int index = string.IsNullOrEmpty(query.Subtype) ? 0 : GetSubtypeIndex(query.Subtype);
			if (index == -1) return null;
			
			var pool = _entriesHash.Where((e, i) => query.GetFilters().All(f => f.Test(dictionary, this, e, i, query)));
			if (!pool.Any()) return null;

			return syncState.GetEntry(query.Carrier, index, pool, rng)?[index];
		}
	}
}