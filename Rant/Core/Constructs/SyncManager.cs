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

namespace Rant.Core.Constructs
{
    internal class SyncManager
    {
        private readonly HashSet<string> _pinQueue = new HashSet<string>();
        private readonly Sandbox _sb;

        private readonly Dictionary<string, Synchronizer> _syncTable =
            new Dictionary<string, Synchronizer>();

        public SyncManager(Sandbox sb)
        {
            _sb = sb;
        }

        public Synchronizer this[string name] => _syncTable[name];

	    public bool SynchronizerExists(string name) => _syncTable.ContainsKey(name);

	    public void Delete(string name) => _syncTable.Remove(name);

        public void Create(string name, SyncType type, bool apply)
        {
            Synchronizer sync;
            if (!_syncTable.TryGetValue(name, out sync))
            {
                sync = _syncTable[name] =
                    new Synchronizer(type, _sb.RNG.NextRaw())
                    {
                        Pinned = _pinQueue.Remove(name)
                    };
            }
            if (apply) _sb.AttribManager.CurrentAttribs.Sync = sync;
        }

        public void Apply(string name)
        {
            Synchronizer sync;
            if (_syncTable.TryGetValue(name, out sync))
                _sb.AttribManager.CurrentAttribs.Sync = sync;
        }

        public void SetPinned(string name, bool isPinned)
        {
            Synchronizer sync;
            if (_syncTable.TryGetValue(name, out sync))
                sync.Pinned = isPinned;
            else if (isPinned)
                _pinQueue.Add(name);
            else
                _pinQueue.Remove(name);
        }

        public void Step(string name)
        {
            Synchronizer sync;
            if (_syncTable.TryGetValue(name, out sync))
                sync.Step(true);
        }

        public void Reset(string name)
        {
            Synchronizer sync;
            if (_syncTable.TryGetValue(name, out sync))
                sync.Reset();
        }
    }
}