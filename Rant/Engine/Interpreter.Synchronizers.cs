using System.Collections.Generic;

namespace Rant
{
    internal partial class Interpreter
    {
        private readonly HashSet<string> _pinQueue = new HashSet<string>(); 
        private readonly Dictionary<string, Synchronizer> _synchronizers = new Dictionary<string, Synchronizer>(4);

        public void SyncCreateApply(string seed, SyncType type)
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

        public void SyncCreate(string seed, SyncType type)
        {
            Synchronizer sync = _synchronizers[seed] = new Synchronizer(type, RNG.GetRaw(seed.Hash(), RNG.Seed));
            if (_pinQueue.Contains(seed)) sync.Pinned = true;
            _pinQueue.Remove(seed);
        }

        public bool TryGetSynchronizer(string seed, out Synchronizer sync)
        {
            return _synchronizers.TryGetValue(seed, out sync);
        }

        public bool SyncApply(string seed)
        {
            Synchronizer sync;
            if (_synchronizers.TryGetValue(seed, out sync))
            {
                NextAttribs.Sync = sync;
                return true;
            }
            return false;
        }

        public void SyncReset(string seed)
        {
            Synchronizer sync;
            if (_synchronizers.TryGetValue(seed, out sync))
            {
                sync.Reset();
            }
        }

        public void SyncSeed(string id, string seed)
        {
            Synchronizer sync;
            if (_synchronizers.TryGetValue(seed, out sync))
            {
                sync.Reseed(seed);
            }
        }

        public void SyncStep(string seed)
        {
            Synchronizer sync;
            if (_synchronizers.TryGetValue(seed, out sync))
            {
                sync.Step(true);
            }
        }

        public void SyncPin(string seed)
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

        public void SyncUnpin(string seed)
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