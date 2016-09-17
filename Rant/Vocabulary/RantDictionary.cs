using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Core;
using Rant.Core.Utilities;
using Rant.Vocabulary.Querying;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Represents a dictionary that can be loaded and queried by Rant.
	/// </summary>
	public sealed class RantDictionary
	{
		private readonly HashSet<string> _exposedClasses = new HashSet<string>();
		private readonly Dictionary<string, RantDictionaryTable> _tables = new Dictionary<string, RantDictionaryTable>();

		/// <summary>
		/// Initializes a new instance of the <see cref="RantDictionary" /> class with no tables.
		/// </summary>
		public RantDictionary()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RantDictionary" /> class with the specified set of tables.
		/// </summary>
		/// <param name="tables">The tables to store in the dictionary.</param>
		public RantDictionary(IEnumerable<RantDictionaryTable> tables)
		{
			_tables = new Dictionary<string, RantDictionaryTable>();

			if (tables == null) return;

			RantDictionaryTable table;
			foreach (var list in tables)
			{
				if (_tables.TryGetValue(list.Name, out table))
				{
					table.Merge(list);
				}
				else
				{
					_tables[list.Name] = list;
				}
			}
		}

		/// <summary>
		/// Gets all currently exposed hidden classes.
		/// </summary>
		public IEnumerable<string> IncludedHiddenClasses => _exposedClasses.AsEnumerable();

		/// <summary>
		/// Exposes a hidden class to query results.
		/// </summary>
		/// <param name="hiddenClassName">The name of the hidden class to expose.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void IncludeHiddenClass(string hiddenClassName)
		{
			if (hiddenClassName == null) throw new ArgumentNullException(nameof(hiddenClassName));
			if (Util.ValidateName(hiddenClassName)) _exposedClasses.Add(hiddenClassName);
		}

		/// <summary>
		/// Unexposes a hidden class from query results.
		/// </summary>
		/// <param name="hiddenClassName">The name of the hidden class to unexpose.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void ExcludeHiddenClass(string hiddenClassName)
		{
			if (hiddenClassName == null) throw new ArgumentNullException(nameof(hiddenClassName));
			_exposedClasses.Remove(hiddenClassName);
		}

		/// <summary>
		/// Determines whether the specified class has been manually exposed (overriding hidden status).
		/// </summary>
		/// <param name="className">The name of the class to check.</param>
		/// <returns></returns>
		public bool ClassExposed(string className)
		{
			if (className == null) throw new ArgumentNullException(nameof(className));
			return _exposedClasses.Contains(className);
		}

		/// <summary>
		/// Adds a new <see cref="RantDictionaryTable" /> object to the dictionary.
		/// </summary>
		/// <param name="table">The table to add.</param>
		public void AddTable(RantDictionaryTable table)
		{
			RantDictionaryTable oldTable;
			if (_tables.TryGetValue(table.Name, out oldTable))
			{
				if (ReferenceEquals(table, oldTable)) return;
				oldTable.Merge(table);
			}
			else
			{
				_tables[table.Name] = table;
			}
		}

		/// <summary>
		/// Enumerates the tables contained in the current RantDictionary instance.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RantDictionaryTable> GetTables()
		{
			foreach (var pair in _tables) yield return pair.Value;
		}

		/// <summary>
		/// Queries the RantDictionary according to the specified criteria and returns a random match.
		/// </summary>
		/// <param name="rng">The random number generator to randomize the match with.</param>
		/// <param name="query">The search criteria to use.</param>
		/// <param name="syncState">The state object to use for carrier synchronization.</param>
		/// <returns></returns>
		internal RantDictionaryTerm Query(Sandbox sb, Query query, CarrierState syncState)
		{
			RantDictionaryTable table;
			return !_tables.TryGetValue(query.Name, out table)
				? null
				: table.Query(this, sb, query, syncState);
		}
	}
}