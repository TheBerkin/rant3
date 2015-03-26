using System.Collections.Generic;

namespace Rant.Engine.ObjectModel
{
    /// <summary>
    /// Stores local variables for a VM instance.
    /// </summary>
    internal class ObjectStack
    {
        private int _level;
        private readonly Dictionary<string, RantObject> _locals = new Dictionary<string, RantObject>();
        private readonly List<HashSet<string>> _scopes = new List<HashSet<string>>();    
        private readonly ObjectTable _table;

        public ObjectStack(ObjectTable table)
        {
            _table = table;
        }

        public RantObject this[string name]
        {
            get
            {
                if (!Util.ValidateName(name)) return null;

                RantObject obj;

                if (_level == 0)
                {
                    return _table.Globals.TryGetValue(name, out obj) ? obj : null;
                }
                return _locals.TryGetValue(name, out obj) ? obj : null;
            }
            set
            {
                if (!Util.ValidateName(name)) return;

                if (value == null)
                {
                    if (_level == 0)
                    {
                        _table.Globals.Remove(name);
                    }
                    else
                    {
                        _locals.Remove(name);
                    }
                    return;
                }

                if (_level == 0)
                {
                    _table.Globals[name] = value;
                }
                else
                {
                    if (!_locals.ContainsKey(name)) _scopes[_level - 1].Add(name);
                    _locals[name] = value;
                }
            }
        }

        public void EnterScope()
        {
            if (++_level > _scopes.Count)
            {
                _scopes.Add(new HashSet<string>());
            }
        }

        public void ExitScope()
        {
            if (_level == 0) return;
            var garbage = _scopes[_level--];
            foreach (var name in garbage)
            {
                _locals.Remove(name);
            }
            garbage.Clear();
        }
    }
}