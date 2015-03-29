using System.Collections.Generic;
using System.Linq;

namespace Rant.Engine.Constructs
{
    internal class SubStore
    {
        private readonly Dictionary<_<string, int>, Subroutine> _table;

        public SubStore()
        {
            _table = new Dictionary<_<string, int>, Subroutine>();
        }

        internal void Remove(Subroutine sub)
        {
            _table.Remove(_table.FirstOrDefault(x => x.Value == sub).Key);
        }

        public void Define(string name, Subroutine sub)
        {
            _table[_.Create(name.ToLower().Trim(), sub.ParamCount)] = sub;
        }

        public Subroutine Get(string name, int argc)
        {
            Subroutine sub;
            return !_table.TryGetValue(_.Create(name.ToLower().Trim(), argc), out sub) ? null : sub;
        }
    }
}