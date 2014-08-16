using System;
using System.Collections.Generic;
using System.Linq;

namespace Manhood
{
    /// <summary>
    /// Outlines search criteria for a Manhood dictionary.
    /// </summary>
    internal sealed class Query
    {
        public string Name { get; set; }

        public string Subtype { get; set; }

        public List<Tuple<string, string>> Filters { get; private set; }

        public Query(string name, string subtype, params Tuple<string, string>[] filters)
        {
            Name = name;
            Subtype = subtype;
            Filters = new List<Tuple<string, string>>(filters);
        }

        public static Query Parse(string input)
        {
            var parts = Util.SplitArgs(input.TrimStart('<').TrimEnd('>')).ToArray();

            if (!parts.Any()) return new Query("", "");

            var entryType = parts[0].Split(new[] { '.' }, 2);

            if (entryType.Length > 2) return new Query("", ""); // Ensure there is only two parts max to the entry type

            var query = new Query(entryType[0], entryType.Length == 2 ? entryType[1] : "");

            // Parse arguments into name/value pairs
            foreach (var pair in parts.Skip(1).Select(arg => arg.Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (pair.Length != 2) return null;
                query.Filters.Add(Tuple.Create(pair[0], pair[1]));
            }

            return query;
        }
    }
}