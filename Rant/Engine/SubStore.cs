using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Engine
{
    internal class SubStore
    {
        private readonly Dictionary<Tuple<string, int>, Subroutine> _table;

        public SubStore()
        {
            _table = new Dictionary<Tuple<string, int>, Subroutine>();
        }

        internal void Remove(Subroutine sub)
        {
            _table.Remove(_table.FirstOrDefault(x => x.Value == sub).Key);
        }

        public void Define(string name, Subroutine sub)
        {
            _table[Tuple.Create(name.ToLower().Trim(), sub.ParamCount)] = sub;
        }

        public Subroutine Get(string name, int argc)
        {
            Subroutine sub;
            return !_table.TryGetValue(Tuple.Create(name.ToLower().Trim(), argc), out sub) ? null : sub;
        }
    }
}