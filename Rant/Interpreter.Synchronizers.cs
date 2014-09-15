using System.Collections.Generic;

namespace Rant
{
    internal partial class Interpreter
    {
        private readonly HashSet<string> _pinQueue = new HashSet<string>(); 
        private readonly Dictionary<string, Synchronizer> _synchronizers = new Dictionary<string, Synchronizer>(4);

        public void Sync(string seed, SyncType type)
        {
            Synchronizer sync;
            if (!_synchronizers.TryGetValue(seed, out sync))
            {
                sync = _synchronizers[seed] = new Synchronizer(type, RNG.GetRaw(seed.Hash(), RNG.Seed));
                if (_pinQueue.Contains(seed)) sync.Pinned = true;
                _pinQueue.Remove(seed);
            }

            NextAttribs.Sync = sync;
        }

        public void Reset(string seed)
        {
            Synchronizer sync;
            if (_synchronizers.TryGetValue(seed, out sync))
            {
                sync.Reset();
            }
        }

        public void Step(string seed)
        {
            Synchronizer sync;
            if (_synchronizers.TryGetValue(seed, out sync))
            {
                sync.Step(true);
            }
        }

        public void Pin(string seed)
        {
            Synchronizer sync;
            if (!_synchronizers.TryGetValue(seed, out sync))
            {
                _pinQueue.Add(seed);
            }
            else
            {
                sync.Pinned = true;
            }
        }

        public void Unpin(string seed)
        {
            Synchronizer sync;
            if (_synchronizers.TryGetValue(seed, out sync))
            {
                sync.Pinned = false;
            }
        }

        public void Desync()
        {
            NextAttribs.Sync = null;
        }
    }
}