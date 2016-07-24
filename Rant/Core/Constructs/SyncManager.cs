using System.Collections.Generic;

namespace Rant.Core.Constructs
{
	internal class SyncManager
	{
		private readonly Dictionary<string, Synchronizer> _syncTable =
			new Dictionary<string, Synchronizer>();
		private readonly HashSet<string> _pinQueue = new HashSet<string>(); 

		private readonly Sandbox _sb;

        public bool SynchronizerExists(string name) => _syncTable.ContainsKey(name);

		public SyncManager(Sandbox sb)
		{
			_sb = sb;
		}

		public void Create(string name, SyncType type, bool apply)
		{
			Synchronizer sync;
			if (!_syncTable.TryGetValue(name, out sync))
				sync = _syncTable[name] = 
					new Synchronizer(type, _sb.RNG.NextRaw())
					{
						Pinned = _pinQueue.Remove(name)
					};
			if (apply) _sb.CurrentBlockAttribs.Sync = sync;
		}

		public void Apply(string name)
		{
			Synchronizer sync;
			if (_syncTable.TryGetValue(name, out sync))
				_sb.CurrentBlockAttribs.Sync = sync;
		}

		public void SetPinned(string name, bool isPinned)
		{
			Synchronizer sync;
			if (_syncTable.TryGetValue(name, out sync))
			{
				sync.Pinned = isPinned;
			}
			else if (isPinned)
			{
				_pinQueue.Add(name);
			}
			else
			{
				_pinQueue.Remove(name);
			}
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

        public Synchronizer this[string name]
        {
            get
            {
                return _syncTable[name];
            }
        }
	}
}