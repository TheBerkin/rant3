using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant
{
    /// <summary>
    /// Represents a set of search criteria for a Rant dictionary.
    /// </summary>
    public class Query
    {
        private string _name;
        private string _subtype;
        private string _carrier;
        private bool _exclusive;
        private readonly List<Tuple<bool, string>[]> _classFilters;
        private readonly List<Tuple<bool, Regex>> _regexFilters;

        public Query(string name, string subtype, string carrier, bool exclusive, IEnumerable<Tuple<bool, string>[]> classFilters,
            IEnumerable<Tuple<bool, Regex>> regexFilters)
        {
            _name = name;
            _subtype = subtype;
            _exclusive = exclusive;
            _carrier = carrier;
            _classFilters = new List<Tuple<bool, string>[]>(classFilters ?? Enumerable.Empty<Tuple<bool, string>[]>());
            _regexFilters = new List<Tuple<bool, Regex>>(regexFilters ?? Enumerable.Empty<Tuple<bool, Regex>>());
        }

        public string Carrier
        {
            get { return _carrier; }
            set { _carrier = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Subtype
        {
            get { return _subtype; }
            set { _subtype = value; }
        }

        public bool Exclusive
        {
            get { return _exclusive; }
            set { _exclusive = value; }
        }

        public List<Tuple<bool, string>[]> ClassFilters
        {
            get { return _classFilters; }
        }

        public List<Tuple<bool, Regex>> RegexFilters
        {
            get { return _regexFilters; }
        }
    }
}