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

		private readonly string _name;
		private readonly string _language = "en_US";
		private readonly string[] _subtypes;
		private readonly HashSet<string> _hidden = new HashSet<string>();
		private readonly HashSet<RantDictionaryEntry> _entriesHash = new HashSet<RantDictionaryEntry>();
		private readonly List<RantDictionaryEntry> _entriesList = new List<RantDictionaryEntry>();

		/// <summary>
		/// Creates a new RantDictionaryTable with the specified entries.
		/// </summary>
		/// <param name="name">the name of the table.</param>
		/// <param name="subtypes">The subtype names.</param>
		/// <param name="entries">The entries to add to the table.</param>
		public RantDictionaryTable(string name, string[] subtypes, IEnumerable<RantDictionaryEntry> entries)
		{
			if (subtypes == null) throw new ArgumentNullException(nameof(subtypes));
			if (entries == null) throw new ArgumentNullException(nameof(entries));

			if (!Util.ValidateName(name))
				throw new FormatException($"Invalid table name: '{name}'");

			if (!subtypes.All(Util.ValidateName))
				throw new FormatException("Invalid subtype name(s): " +
										  String.Join(", ",
											  subtypes.Where(s => !Util.ValidateName(s)).Select(s => $"'{s}'").ToArray()));

			_subtypes = subtypes;
			_name = name;
			_entriesHash.AddRange(entries);
			_entriesList.AddRange(entries);
		}

		/// <summary>
		/// Creates a new RantDictionaryTable with the specified entries.
		/// </summary>
		/// <param name="name">the name of the table.</param>
		/// <param name="subtypes">The subtype names.</param>
		/// <param name="entries">The entries to add to the table.</param>
		/// <param name="hiddenClasses">The classes to hide.</param>
		public RantDictionaryTable(string name, string[] subtypes, IEnumerable<RantDictionaryEntry> entries, IEnumerable<string> hiddenClasses)
		{
			if (entries == null) throw new ArgumentNullException(nameof(entries));
			if (hiddenClasses == null) throw new ArgumentNullException(nameof(hiddenClasses));
			if (!Util.ValidateName(name))
				throw new FormatException($"Invalid table name: '{name}'");

			if (!subtypes.All(Util.ValidateName))
				throw new FormatException("Invalid subtype name(s): " +
										  String.Join(", ",
											  subtypes.Where(s => !Util.ValidateName(s)).Select(s => $"'{s}'").ToArray()));

			_subtypes = subtypes;
			_name = name;
			_entriesHash.AddRange(entries);
			_entriesList.AddRange(entries);
			foreach (var hiddenClass in hiddenClasses.Where(Util.ValidateName)) _hidden.Add(hiddenClass);
		}

		/// <summary>
		/// Gets the entries stored in the table.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RantDictionaryEntry> GetEntries()
		{
			foreach (var entry in _entriesHash) yield return entry;
		}

		/// <summary>
		/// Adds the specified entry to the table.
		/// </summary>
		/// <param name="entry">The entry to add to the table.</param>
		/// <returns>True if successfully added; otherwise, False.</returns>
		public bool AddEntry(RantDictionaryEntry entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
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

			foreach (var c in _entriesHash.SelectMany(e => e.GetClasses()))
			{
				if (lstClasses.Add(c)) yield return c;
			}
		}

		/// <summary>
		/// The subtypes used by the table entries.
		/// </summary>
		public string[] Subtypes => _subtypes;

		/// <summary>
		/// The name of the table.
		/// </summary>
		public string Name => _name;

		/// <summary>
		/// The language of the table (not yet used).
		/// </summary>
		public string Language => _language;

		/// <summary>
		/// Gets the hidden classes of the table.
		/// </summary>
		public IEnumerable<string> HiddenClasses => _hidden;

		/// <summary>
		/// The number of entries stored in the table.
		/// </summary>
		public int EntryCount => _entriesHash.Count;

		private int GetSubtypeIndex(string subtype)
		{
			if (String.IsNullOrEmpty(subtype)) return 0;
			for (int i = 0; i < _subtypes.Length; i++)
			{
				if (String.Equals(subtype, _subtypes[i], StringComparison.OrdinalIgnoreCase)) return i;
			}
			return -1;
		}

		/// <summary>
		/// Adds another table's entries to the current table, given that they share the same name and subtypes.
		/// </summary>
		/// <param name="other">The table whose entries will be added to the current instance.</param>
		/// <returns>True if merge succeeded; otherwise, False.</returns>
		public bool Merge(RantDictionaryTable other)
		{
			if (other._name != _name || other == this) return false;
			if (!other._subtypes.SequenceEqual(_subtypes)) return false;
			_entriesHash.AddRange(other._entriesHash);
			_entriesList.AddRange(other._entriesHash);
			return true;
		}

		internal string Query(RantDictionary dictionary, RNG rng, Query query, CarrierState syncState)
		{
			var index = String.IsNullOrEmpty(query.Subtype) ? 0 : GetSubtypeIndex(query.Subtype);
			if (index == -1) return "[Bad Subtype]";

			var pool = query.ClassFilter.IsEmpty
				? _entriesHash
				: _entriesHash.Where(e => query.ClassFilter.Test(e, query.Exclusive));

			if (_hidden.Any())
			{
				var hide = _hidden.Where(hc => !query.ClassFilter.AllowsClass(hc))
					.Except(dictionary.IncludedHiddenClasses);
				pool = pool.Where(e => !hide.Any(e.ContainsClass));
			}

			if (query.RegexFilters.Any())
				pool = query.RegexFilters.Aggregate(pool, (current, regex) => current.Where(e => regex.Item1 == regex.Item2.IsMatch(e[index].Value)));

			if (query.SyllablePredicate != null)
				pool = pool.Where(e => query.SyllablePredicate.Test(e[index].SyllableCount));

			if (!pool.Any()) return MissingTerm;

			return syncState.GetEntry(query.Carrier, index, pool, rng)?[index].Value ?? MissingTerm;
		}
	}
}