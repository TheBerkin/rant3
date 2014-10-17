using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant
{
    /// <summary>
    /// Represents a set of search criteria for a Rant dictionary.
    /// </summary>
    public sealed class Query
    {
        private readonly string _name;
        private readonly string _subtype;
        private readonly string _carrier;
        private readonly bool _exclusive;
        private readonly IEnumerable<Tuple<bool, string>[]> _classFilters;
        private readonly IEnumerable<Tuple<bool, Regex>> _regexFilters;

        /// <summary>
        /// Creates a new Query object with the specified arguments.
        /// </summary>
        /// <param name="name">The name of the dictionary to search.</param>
        /// <param name="subtype">The subtype of the dictionary entry to use.</param>
        /// <param name="carrier">The carrier name to synchronize selections with. Leave blank for no carrier.</param>
        /// <param name="exclusive">Specifies exclusivity of class filters.</param>
        /// <param name="classFilters">The class filters to search by.</param>
        /// <param name="regexFilters">The regex filters to search by.</param>
        public Query(string name, string subtype, string carrier, bool exclusive, IEnumerable<Tuple<bool, string>[]> classFilters,
            IEnumerable<Tuple<bool, Regex>> regexFilters)
        {
            _name = name;
            _subtype = subtype;
            _exclusive = exclusive;
            _carrier = carrier;
            _classFilters = (classFilters ?? Enumerable.Empty<Tuple<bool, string>[]>()).AsEnumerable();
            _regexFilters = (regexFilters ?? Enumerable.Empty<Tuple<bool, Regex>>()).AsEnumerable();
        }

        /// <summary>
        /// The carrier name to synchronize selections with.
        /// </summary>
        public string Carrier
        {
            get { return _carrier; }
        }

        /// <summary>
        /// The name of the dictionary to search.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The subtype of the dictionary entry to use.
        /// </summary>
        public string Subtype
        {
            get { return _subtype; }
        }

        /// <summary>
        /// Specifies exclusivity of class filters.
        /// </summary>
        public bool Exclusive
        {
            get { return _exclusive; }
        }

        /// <summary>
        /// The class filters to search by.
        /// </summary>
        public IEnumerable<Tuple<bool, string>[]> ClassFilters
        {
            get { return _classFilters; }
        }

        /// <summary>
        /// The regex filters to search by.
        /// </summary>
        public IEnumerable<Tuple<bool, Regex>> RegexFilters
        {
            get { return _regexFilters; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 486187739 + (_name ?? "").GetHashCode();
                hash = hash * 486187739 + (_subtype ?? "").GetHashCode();
                hash = hash * 486187739 + (_carrier ?? "").GetHashCode();
                hash = hash * 486187739 + (_exclusive ? 13 : 17);
                hash = hash * 486187739 +
                    _classFilters.Aggregate(hash, (c, n) =>
                        c + n.Aggregate(n.GetHashCode(), (cc, nn) =>
                            cc + nn.GetHashCode() * 486187739 + 17)
                        * 486187739 + 17);
                hash = hash * 486187739 + _regexFilters.Aggregate(hash, (c, n) =>
                    (c + n.Item1.GetHashCode() + n.Item2.ToString().GetHashCode() + n.Item2.Options.GetHashCode()) * 486187739 + 17);
                return hash;
            }
        }
    }
}