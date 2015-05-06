using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary
{
    internal class EntryTypeDef
    {
        private readonly HashSet<string> _classes;
        private readonly HashSet<string> _testTable = new HashSet<string>(); 

        public string Name { get; }

        public EntryTypeDefFilter Filter { get; }

        public IEnumerator<string> GetTypeClasses() => _classes.AsEnumerable().GetEnumerator();

        public EntryTypeDef(string name, IEnumerable<string> classes, EntryTypeDefFilter filter)
        {
            Name = name;
            _classes = new HashSet<string>();
            foreach (var c in classes) _classes.Add(c);
            Filter = filter;
        }

        public bool IsValidValue(string value) => _classes.Contains(value);

        public bool Test(RantDictionaryEntry entry)
        {
            if (!EntryTypeDefFilter.Test(Filter, entry)) return true;
            _testTable.Clear();
            foreach (var c in entry.GetClasses().Where(IsValidValue))
            {
                if (_testTable.Add(c) && _testTable.Any()) return false;
            }
            return true;
        }
    }
}