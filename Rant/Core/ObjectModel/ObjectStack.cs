#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections.Generic;

using Rant.Core.Utilities;

namespace Rant.Core.ObjectModel
{
    /// <summary>
    /// Stores local variables for a VM instance.
    /// </summary>
    internal class ObjectStack
    {
        private readonly List<HashSet<string>> _scopes = new List<HashSet<string>>();
        private readonly ObjectTable _table;
        private int _level;

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

                if (_table.Globals.TryGetValue(name, out obj)) return obj;
                return CurrentLocals.TryGetValue(name, out obj) ? obj : null;
            }
            set
            {
                if (!Util.ValidateName(name)) return;

                if (value == null)
                {
                    if (_level == 0)
                        _table.Globals.Remove(name);
                    else
                        CurrentLocals.Remove(name);
                    return;
                }

                if (_level == 0)
                    _table.Globals[name] = value;
                else
                {
                    if (_table.Globals.ContainsKey(name))
                        _table.Globals[name] = value;
                    if (!CurrentLocals.ContainsKey(name))
                        _scopes[_level - 1].Add(name);
                    CurrentLocals[name] = value;
                }
            }
        }

        public Dictionary<string, RantObject> CurrentLocals { get; } = new Dictionary<string, RantObject>();

        public void EnterScope()
        {
            if (++_level >= _scopes.Count)
                _scopes.Add(new HashSet<string>());
        }

        public void ExitScope()
        {
            if (_level == 0) return;
            var garbage = _scopes[--_level];
            foreach (string name in garbage)
                CurrentLocals.Remove(name);
            garbage.Clear();
        }

        public void Clear()
        {
            CurrentLocals.Clear();
            _scopes.Clear();
            _level = 0;
            _table.Globals.Clear();
        }
    }
}