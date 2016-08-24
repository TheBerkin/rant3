using System;

using Rant.Core.Utilities;

namespace Rant.Core.Constructs
{
	internal class Synchronizer
	{
		private readonly RNG _rng;
		private bool _bounce;
		private int[] _state;

		public Synchronizer(SyncType type, long seed)
		{
			Type = type;
			_rng = new RNG(seed);
			Index = 0;
			_state = null;
			Pinned = false;
		}

		public bool Pinned { get; set; }
		public SyncType Type { get; set; }
		public int Index { get; set; }

		public void Reset()
		{
			_rng.Reset();
			Index = 0;
			if (_state == null) return;
			FillSlots();
		}

		public void Reseed(string id)
		{
			_rng.Reset(id.Hash());
			Index = 0;
			if (_state == null) return;
			FillSlots();
		}

		public int NextItem(int count)
		{
			if (_state == null)
			{
				_state = new int[count];
				FillSlots();
			}
			else if (count != _state.Length)
			{
				Array.Resize(ref _state, count);
				FillSlots();
			}

			return Step(false);
		}

		public int Step(bool force)
		{
			if (Type == SyncType.Locked) return _state[0];
			if (Index >= _state.Length)
			{
				switch (Type)
				{
					case SyncType.Deck:
						ScrambleSlots();
						goto default;
					case SyncType.Ping:
					case SyncType.Pong:
						_bounce = !_bounce;
						Index = 1;
						break;
					default:
						Index = 0;
						break;
				}
			}
			if (Pinned && !force) return _state[Index];

			switch (Type)
			{
				case SyncType.Ping:
				case SyncType.Pong:
					return _bounce ? _state[(_state.Length - 1) - Index++] : _state[Index++];
				default:
					return _state[Index++];
			}
		}

		private void FillSlots()
		{
			switch (Type)
			{
				case SyncType.Forward:
				case SyncType.Ping:
					for (int i = 0; i < _state.Length; _state[i] = i++)
					{
					}
					break;
				case SyncType.Reverse:
				case SyncType.Pong:
					for (int i = 0; i < _state.Length; _state[(_state.Length - 1) - i] = i++)
					{
					}
					break;
				case SyncType.Locked:
				case SyncType.Deck:
				case SyncType.Cdeck:
					for (int i = 0; i < _state.Length; _state[i] = i++)
					{
					}
					ScrambleSlots();
					break;
			}
		}

		private void ScrambleSlots()
		{
			if (_state.Length == 1) return;

			int t;

			if (_state.Length == 2) // Handle 2-item scenario
			{
				if (_rng.Next(0, 2) != 0) return;
				t = _state[0];
				_state[0] = _state[1];
				_state[1] = t;
				return;
			}

			int s;

			for (int i = 0; i < _state.Length; i++)
			{
				t = _state[i];
				do
				{
					s = _rng.Next(_state.Length);
				} while (s == t && _state.Length < 3);
				_state[i] = _state[s];
				_state[s] = t;
			}
		}
	}
}