#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Core;
using Rant.Core.IO.Bson;
using Rant.Core.Utilities;
using Rant.Resources;
using Rant.Vocabulary.Utilities;
using Rant.Vocabulary.Querying;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Represents a named collection of dictionary entries.
	/// </summary>
	public sealed partial class RantDictionaryTable : RantResource
	{
		internal const string NSFW = "nsfw";
		internal const string MissingTerm = "[?]";
		private readonly HashSet<RantDictionaryEntry> _entriesHash = new HashSet<RantDictionaryEntry>();
		private readonly List<RantDictionaryEntry> _entriesList = new List<RantDictionaryEntry>(); // TODO: Use for indexing / weighted selection
		private readonly HashSet<string> _hidden = new HashSet<string>(new[] { NSFW });
		private readonly Dictionary<int, HashSet<string>> _subtypeIndexMap = new Dictionary<int, HashSet<string>>();
		private readonly Dictionary<string, int> _subtypes = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
		private ClassTree _classTree = new ClassTree(new RantDictionaryEntry[] { });
		private SyllableBuckets[] _syllableBuckets;

		/// <summary>
		/// Initializes a new instance of the RantDictionaryTable class with the specified name and term count.
		/// </summary>
		/// <param name="name">The name of the table.</param>
		/// <param name="termsPerEntry">The number of terms to store in each entry.</param>
		public RantDictionaryTable(string name, int termsPerEntry, HashSet<string> hidden = null)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (termsPerEntry <= 0) throw new ArgumentException("Terms per entry must be greater than zero.");
			if (!Util.ValidateName(name)) throw new ArgumentException($"Invalid table name: '{name}'");
			if (hidden != null) _hidden = hidden;
			TermsPerEntry = termsPerEntry;
			Name = name;

			CreateSyllableBuckets();
		}

		internal RantDictionaryTable()
		{
			// Used by serializer
		}

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the language code associated with the table (not yet used).
		/// </summary>
		public string Language { get; private set; } = "en-US";

		/// <summary>
		/// Gets the hidden classes of the table.
		/// </summary>
		public IEnumerable<string> HiddenClasses => _hidden.AsEnumerable();

		/// <summary>
		/// Gets the number of entries stored in the table.
		/// </summary>
		public int EntryCount => _entriesHash.Count;

		/// <summary>
		/// Gets the number of terms required for entries contained in the current table.
		/// </summary>
		public int TermsPerEntry { get; private set; }

		/// <summary>
		/// Enumerates the entries stored in the table.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RantDictionaryEntry> GetEntries()
		{
			foreach (var entry in _entriesHash) yield return entry;
		}

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
			if (entry.TermCount != TermsPerEntry) return false;
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
				if (lstClasses.Add(c)) yield return c;
		}

		/// <summary>
		/// Adds a subtype of the specified name to the table.
		/// If a subtype with the name already exists, it will be overwritten.
		/// Subtypes are case insensitive.
		/// If the name is not a valid identifier string, it will not be accepted.
		/// </summary>
		/// <param name="subtypeName">The name of the subtype to add.</param>
		/// <param name="index">The term index to associate with the name.</param>
		/// <returns>
		/// FALSE if the name was not a valid identifier or the index was out of range. TRUE if the operation was
		/// successful.
		/// </returns>
		public bool AddSubtype(string subtypeName, int index)
		{
			if (index < 0 || index >= TermsPerEntry) return false;
			if (subtypeName == null) throw new ArgumentNullException(nameof(subtypeName));
			if (!Util.ValidateName(subtypeName)) return false;
			_subtypes[subtypeName] = index;
			HashSet<string> subs;
			if (!_subtypeIndexMap.TryGetValue(index, out subs))
				_subtypeIndexMap[index] = subs = new HashSet<string>();
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
				return subs.Remove(subtypeName) && _subtypes.Remove(subtypeName);
			return false;
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
			if (index < 0 || index >= TermsPerEntry) yield break;
			HashSet<string> subs;
			if (!_subtypeIndexMap.TryGetValue(index, out subs)) yield break;
			foreach (string sub in subs) yield return sub;
		}

		/// <summary>
		/// Adds another table's entries to the current table, given that they share the same name and term count.
		/// </summary>
		/// <param name="other">The table whose entries will be added to the current instance.</param>
		/// <returns>True if merge succeeded; otherwise, False.</returns>
		public bool Merge(RantDictionaryTable other)
		{
			if (other.Name != Name || other == this) return false;
			if (other.TermsPerEntry != TermsPerEntry) return false;
			_entriesHash.AddRange(other._entriesHash);
			_entriesList.AddRange(other._entriesHash);
			return true;
		}

		/// <summary>
		/// Optimizes the table. Call this after writing items to the table or removing items from a table.
		/// If you're writing or removing multiple items, call this after all the actions have been performed.
		/// </summary>
		public void Commit()
		{
			_classTree = new ClassTree(_entriesHash);
			CreateSyllableBuckets();
			for (var i = 0; i < TermsPerEntry; i++)
			{
				_syllableBuckets[i] = new SyllableBuckets(i, _entriesList);
			}
		}

		internal RantDictionaryTerm Query(RantDictionary dictionary, Sandbox sb, Query query, CarrierState syncState)
		{
            int index = !String.IsNullOrEmpty(query.PluralSubtype) && sb.TakePlural()
                ? GetSubtypeIndex(query.PluralSubtype)
                : GetSubtypeIndex(query.Subtype);

			if (index == -1) return null;

			if (query.BareQuery) return _entriesList.PickEntry(sb.RNG)?[index];

			// process simple class filters using class tree
			var filters = query.GetFilters();
			var classes = filters
				.Where(f => f is ClassFilter)
				.SelectMany(f => (f as ClassFilter).RequiredClasses);

			filters = filters.Where(f => !(f is ClassFilter) || !(f as ClassFilter).SimpleFilter);

			IEnumerable<RantDictionaryEntry> pool = _entriesList;
			if (classes.Any())
			{
				pool = _classTree.Query(classes);
			}

			// if it's just the class filters, let's leave now
			if (!filters.Any() && !query.HasCarrier)
			{
				return pool.ToList().PickEntry(sb.RNG)?[index];
			}

			// process syllable count filters using syllable buckets
			var rangeFilters = filters.Where(f => f is RangeFilter).Select(f => f as RangeFilter);
			if (rangeFilters.Any())
			{
				CreateSyllableBuckets();

				foreach (var filter in rangeFilters)
				{
					pool = pool.Intersect(_syllableBuckets[index].Query(filter));
				}

				filters = filters.Where(f => !(f is RangeFilter));
			}

			if (filters.Any())
			{
				pool = pool.Where((e, i) => filters.OrderBy(f => f.Priority).All(f => f.Test(dictionary, this, e, index, query)));
			}

			if (!pool.Any()) return null;

			if (query.HasCarrier)
			{
				return syncState.GetEntry(query.Carrier, index, pool, sb.RNG)?[index];
			}

			return pool.PickEntry(sb.RNG)?[index];
		}

		internal override void DeserializeData(BsonItem data)
		{
			// General data
			Name = data["name"];
			TermsPerEntry = data["termc"];
			Language = data["lang"];

			// Subtypes
			var subs = data["subs"].Values;
			int si = 0;
			foreach (var subList in subs)
			{
				foreach (var sub in subList.Values) AddSubtype(sub, si);
				si++;
			}

			// Hidden classes
			foreach (var hiddenClass in data["hidden-classes"].Values) _hidden.Add(hiddenClass);

			var entries = data["entries"];
			int count = entries.Count;

			// Entries
			for (int i = 0; i < count; i++)
			{
				var entryData = entries[i];

				// Terms
				var terms =
					from BsonItem termData in entryData["terms"].Values
					let value = (string)termData["value"]
					let pron = (string)termData["pron"]
					let valueSplit = (int)(termData["value-split"] ?? -1)
					let pronSplit = (int)(termData["pron-split"] ?? -1)
					select new RantDictionaryTerm(value, pron, valueSplit, pronSplit);

				var entryClasses = (string[])entryData["classes"];
				var termArray = terms.ToArray();
				var entry = new RantDictionaryEntry(termArray, entryClasses, entryData["weight"] ?? 1);

				// Optional classes
				foreach (var optionalClass in entryData["optional-classes"].Values)
					entry.AddClass(optionalClass, true);

				// Metadata
				var meta = entryData["metadata"];
				foreach (string metaKey in meta.Keys)
					entry.SetMetadata(metaKey, meta[metaKey].Value);

				AddEntry(entry);
			}

			Commit();
		}

		internal override BsonItem SerializeData()
		{
			var data = new BsonItem
			{
				["name"] = Name,
				["lang"] = Language,
				["termc"] = TermsPerEntry
			};

			var subs = new BsonItem[TermsPerEntry];
			for (int i = 0; i < TermsPerEntry; i++)
				subs[i] = new BsonItem(GetSubtypesForIndex(i).ToArray());
			data["subs"] = new BsonItem(subs);

			data["hidden-classes"] = new BsonItem(_hidden.ToArray());

			var entries = new BsonItem[_entriesList.Count];
			for (int i = 0; i < _entriesList.Count; i++)
			{
				var entry = _entriesList[i];
				var entryData = new BsonItem();
				var termData = new BsonItem[TermsPerEntry];

				for (int j = 0; j < TermsPerEntry; j++)
					termData[j] = new BsonItem
					{
						["value"] = entry[j].Value,
						["pron"] = entry[j].Pronunciation,
						["value-split"] = entry[j].ValueSplitIndex,
						["pron-split"] = entry[j].PronunciationSplitIndex
					};
				entryData["terms"] = new BsonItem(termData);

				entryData["classes"] = new BsonItem(entry.GetRequiredClasses().ToArray());
				entryData["optional-classes"] = new BsonItem(entry.GetOptionalClasses().ToArray());

				var metaData = new BsonItem();
				foreach (string metaKey in entry.GetMetadataKeys())
					metaData[metaKey] = new BsonItem(entry.GetMetadata(metaKey));
				entryData["metadata"] = metaData;
				entries[i] = entryData;
			}

			data["entries"] = new BsonItem(entries);

			return data;
		}

		internal override void Load(RantEngine engine)
		{
			engine.Dictionary.AddTable(this);
		}

		internal void CreateSyllableBuckets()
		{
			if (_syllableBuckets != null) return;
			_syllableBuckets = new SyllableBuckets[TermsPerEntry];
			for (var i = 0; i < TermsPerEntry; i++)
			{
				_syllableBuckets[i] = new SyllableBuckets(i, new RantDictionaryEntry[] { });
			}
		}
	}
}