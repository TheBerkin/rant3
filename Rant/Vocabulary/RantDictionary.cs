using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant.Core.Utilities;
using Rant.Vocabulary.Querying;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Represents a Rant dictionary.
    /// </summary>
    public sealed class RantDictionary
    {
        private readonly Dictionary<string, RantDictionaryTable> _tables = new Dictionary<string, RantDictionaryTable>();
        private readonly HashSet<string> _exposedClasses = new HashSet<string>();

        /// <summary>
        /// Creates a new RantDictionary object that contains no tables.
        /// </summary>
        public RantDictionary()
        {
        }

        /// <summary>
        /// Creates a new RantDictionary object from the specified dictionary collection.
        /// </summary>
        /// <param name="tables">The tables to store in the dictionary.</param>
        /// <param name="mergeBehavior">The merging strategy to employ.</param>
        public RantDictionary(IEnumerable<RantDictionaryTable> tables, TableMergeBehavior mergeBehavior = TableMergeBehavior.Naive)
        {
            _tables = new Dictionary<string, RantDictionaryTable>();

            if (tables == null) return;

            RantDictionaryTable table;
            foreach (var list in tables)
            {
                if (_tables.TryGetValue(list.Name, out table))
                {
                    table.Merge(list, mergeBehavior);
                }
                else
                {
                    _tables[list.Name] = list;
                }
            }
        }

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
        /// Gets all currently exposed hidden classes.
        /// </summary>
        public IEnumerable<string> IncludedHiddenClasses => _exposedClasses.AsEnumerable();

        /// <summary>
        /// Adds a new RantDictionaryTable object to the collection.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="mergeBehavior">The merging strategy to employ.</param>
        public void AddTable(RantDictionaryTable table, TableMergeBehavior mergeBehavior = TableMergeBehavior.Naive)
        {
            RantDictionaryTable oldTable;
            if (_tables.TryGetValue(table.Name, out oldTable))
            {
                oldTable.Merge(table, mergeBehavior);
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
        /// Loads all dictionary (.dic) files from the specified directory and returns a RantDictionary object that contains the loaded data.
        /// </summary>
        /// <param name="directory">The directory from which to load dictionaries.</param>
        /// <param name="mergeBehavior">The merging strategy to employ.</param>
        /// <returns></returns>
        public static RantDictionary FromDirectory(string directory, TableMergeBehavior mergeBehavior = TableMergeBehavior.Naive)
        {
            return new RantDictionary(Directory.GetFiles(directory, "*.dic", SearchOption.AllDirectories).Select(RantDictionaryTable.FromFile).ToList(), mergeBehavior);
        }

        /// <summary>
        /// Loads all dictionary (.dic) files from the specified directories and returns a RantDictionary object that contains the loaded data.
        /// </summary>
        /// <param name="directories">The directories from which to load dictionaries.</param>
        /// <returns></returns>
        public static RantDictionary FromMultiDirectory(params string[] directories)
        {
            return new RantDictionary(directories.SelectMany(path => Directory.GetFiles(path, "*.dic", SearchOption.AllDirectories)).Select(RantDictionaryTable.FromFile));
        }

        /// <summary>
        /// Loads all dictionary (.dic) files from the specified directories and returns a RantDictionary object that contains the loaded data.
        /// </summary>
        /// <param name="directories">The directories from which to load dictionaries.</param>
        /// <param name="mergeBehavior">The merging strategy to employ.</param>
        /// <returns></returns>
        public static RantDictionary FromMultiDirectory(string[] directories, TableMergeBehavior mergeBehavior)
        {
            return new RantDictionary(directories.SelectMany(path => Directory.GetFiles(path, "*.dic", SearchOption.AllDirectories)).Select(RantDictionaryTable.FromFile), mergeBehavior);
        }

        /// <summary>
        /// Queries the RantDictionary according to the specified criteria and returns a random match.
        /// </summary>
        /// <param name="rng">The random number generator to randomize the match with.</param>
        /// <param name="query">The search criteria to use.</param>
        /// <param name="syncState">The state object to use for carrier synchronization.</param>
        /// <returns></returns>
        internal string Query(RNG rng, Query query, QueryState syncState)
        {
            RantDictionaryTable table;
            return !_tables.TryGetValue(query.Name, out table) 
                ? "[Missing Table]" 
                : table.Query(this, rng, query, syncState);
        }
    }
}