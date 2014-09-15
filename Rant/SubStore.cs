using System;
using System.Collections.Generic;

namespace Rant
{
    internal class SubStore
    {
        private readonly Dictionary<Tuple<string, int>, Subroutine> _table;

        public SubStore()
        {
            _table = new Dictionary<Tuple<string, int>, Subroutine>();
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