using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using Rant.IO;
using Rant.Vocabulary;

namespace Rant
{
    /// <summary>
    /// Represents a collection of patterns and tables that can be exported to an archive file.
    /// </summary>
    public sealed class RantPackage
    {
        private const string Magic = "RPKG";

        private HashSet<RantPattern> _patterns;
        private HashSet<RantDictionaryTable> _tables;

        /// <summary>
        /// Adds the specified pattern to the package.
        /// </summary>
        /// <param name="pattern">The pattern to add.</param>
        public void AddPattern(RantPattern pattern)
            => (_patterns ?? (_patterns = new HashSet<RantPattern>())).Add(pattern);

        /// <summary>
        /// Adds the specified table to the package.
        /// </summary>
        /// <param name="table">The table to add.</param>
        public void AddTable(RantDictionaryTable table)
            => (_tables ?? (_tables = new HashSet<RantDictionaryTable>())).Add(table);

        /// <summary>
        /// Adds the tables from the specified dictionary to the package.
        /// </summary>
        /// <param name="dictionary">The dictionary to add.</param>
        public void AddDictionary(RantDictionary dictionary)
        {
            if (_tables == null) 
                _tables = new HashSet<RantDictionaryTable>();

            foreach (var table in dictionary.GetTables())
            {
                _tables.Add(table);
            }
        }

        /// <summary>
        /// Enumerates the patterns contained in the package.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RantPattern> GetPatterns()
        {
            if (_patterns == null) yield break;
            foreach (var pattern in _patterns)
                yield return pattern;
        }

        /// <summary>
        /// Enumerates the tables contained in the package.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RantDictionaryTable> GetTables()
        {
            if (_tables == null) yield break;
            foreach (var table in _tables)
                yield return table;
        } 

        /// <summary>
        /// Saves the package to the specified file path.
        /// </summary>
        /// <param name="path">The path to the file to create.</param>
        public void Save(string path)
        {
            if (String.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path += ".rantpkg";
            }

            using (var writer = new EasyWriter(new GZipStream(File.Create(path), CompressionMode.Compress)))
            {
                // Magic
                writer.WriteBytes(Encoding.ASCII.GetBytes(Magic));

                // Counts
                writer.Write(_patterns?.Count ?? 0);
                writer.Write(_tables?.Count ?? 0);

                // Patterns
                if (_patterns != null)
                {
                    foreach (var pattern in _patterns)
                    {
                        writer.Write(pattern.Name);
                        writer.Write(pattern.Code);
                    }
                }

                // Tables
                if (_tables != null)
                {
                    foreach (var table in _tables)
                    {
                        writer
                        .Write(table.Name)
                        .Write(table.Subtypes)
                        .Write(table.EntryCount)
                        .Write(table.HiddenClasses.ToArray());

                        foreach (var entry in table.GetEntries())
                        {
                            writer
                                .Write(entry.Weight)
                                .Write(false) // Used to be the NSFW field, will use for something else in the future!
                                .Write(entry.Terms.Length);

                            for (int i = 0; i < entry.Terms.Length; i++)
                            {
                                writer
                                    .Write(entry.Terms[i].Value)
                                    .Write(entry.Terms[i].Pronunciation);
                            }

                            writer.Write(entry.GetClasses().ToArray());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads a package from the specified path and returns it as a RantPackage object.
        /// </summary>
        /// <param name="path">The path to the package file to load.</param>
        /// <returns></returns>
        public static RantPackage Load(string path)
        {
            if (String.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path += ".rantpkg";
            }

            using (var reader = new EasyReader(new GZipStream(File.Open(path, FileMode.Open), CompressionMode.Decompress)))
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != Magic)
                    throw new InvalidDataException("File is corrupt.");

                int numPatterns = reader.ReadInt32();
                int numTables = reader.ReadInt32();

                if (numPatterns < 0 || numTables < 0)
                    throw new InvalidDataException("File is corrupt.");

                var pkg = new RantPackage();

                // Patterns
                for (int i = 0; i < numPatterns; i++)
                {
                    var name = reader.ReadString();
                    var code = reader.ReadString();

                    pkg.AddPattern(new RantPattern(name, RantPatternSource.String, code));
                }

                // Tables
                for (int i = 0; i < numTables; i++)
                {
                    var name = reader.ReadString();
                    var subs = reader.ReadStringArray();
                    int numEntries = reader.ReadInt32();
                    var hiddenClasses = reader.ReadStringArray();
                    var entries = new RantDictionaryEntry[numEntries];

                    for (int j = 0; j < numEntries; j++)
                    {
                        int weight = reader.ReadInt32();
                        bool flags = reader.ReadBoolean(); // unused
                        int numTerms = reader.ReadInt32();
                        var terms = new RantDictionaryTerm[numTerms];

                        for (int k = 0; k < numTerms; k++)
                        {
                            var value = reader.ReadString();
                            var pron = reader.ReadString();
                            terms[k] = new RantDictionaryTerm(value, pron);
                        }

                        var classes = reader.ReadStringArray();

                        entries[j] = new RantDictionaryEntry(terms, classes, weight);
                    }

                    pkg.AddTable(new RantDictionaryTable(name, subs, entries, hiddenClasses));
                }

                return pkg;
            }
        }
    }
}