using System;

namespace Manhood
{
    internal class Query
    {
        private readonly string _name;
        private readonly string _subtype;
        private readonly bool _exclusive;
        private readonly Tuple<bool, string>[] _classFilters;
        private readonly Tuple<bool, string>[] _regexFilters;

        public Query(string name, string subtype, bool exclusive, Tuple<bool, string>[] classFilters,
            Tuple<bool, string>[] regexFilters)
        {
            _name = name;
            _subtype = subtype;
            _exclusive = exclusive;
            _classFilters = classFilters;
            _regexFilters = regexFilters;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Subtype
        {
            get { return _subtype; }
        }

        public bool Exclusive
        {
            get { return _exclusive; }
        }

        public Tuple<bool, string>[] ClassFilters
        {
            get { return _classFilters; }
        }

        public Tuple<bool, string>[] RegexFilters
        {
            get { return _regexFilters; }
        }
    }
}