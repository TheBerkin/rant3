using System.Collections.Generic;

namespace Manhood
{
    internal class SubStore
    {
        private readonly Dictionary<string, Subroutine> _subs = new Dictionary<string, Subroutine>();

        public SubStore()
        {
            
        }

        public bool Define(string name, Subroutine value)
        {
            if (!Util.ValidateName(name)) return false;
            _subs[name.ToLower()] = value;
            return true;
        }

        public Subroutine GetSubroutine(string name)
        {
            Subroutine sub;
            return !_subs.TryGetValue(name.ToLower(), out sub) ? null : sub;
        }

        public bool Undefine(string name)
        {
            return _subs.Remove(name);
        }
    }
}