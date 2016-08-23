using System;

using Rant.Core.Utilities;

namespace Rant.Core.Constructs
{
	internal class Synchronizer
	{
		private SyncType _type;
		private int _index;
		private int[] _state;
		private bool _pinned, _bounce;
		private readonly RNG _rng;

		public Synchronizer(SyncType type, long seed)
		{
			_type = type;
			_rng = new RNG(seed);
			_index = 0;
			_state = null;
			_pinned = false;
		}

		public bool Pinned
		{
			get { return _pinned; }
			set { _pinned = value; }
		}

		public SyncType Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public void Reset()
		{
			_rng.Reset();
			_index = 0;
			if (_state == null) return;
			FillSlots();
		}

		public void Reseed(string id)
		{
			_rng.Reset(id.Hash());
			_index = 0;
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
			if (_type == SyncType.Locked) return _state[0];
			if (_index >= _state.Length)
			{
				switch (_type)
				{
					case SyncType.Deck:
						ScrambleSlots();
						goto default;
					case SyncType.Ping:
					case SyncType.Pong:
						_bounce = !_bounce;
						_index = 1;
						break;
					default:
						_index = 0;
						break;
				}
			}
			if (_pinned && !force) return _state[_index];

			switch (_type)
			{
				case SyncType.Ping:
				case SyncType.Pong:
					return _bounce ? _state[(_state.Length - 1) - _index++] : _state[_index++];
				default:
					return _state[_index++];
			}
		}

		private void FillSlots()
		{
			switch (_type)
			{
				case SyncType.Ordered:
				case SyncType.Ping:
					for (int i = 0; i < _state.Length; _state[i] = i++) { }
					break;
				case SyncType.Reverse:
				case SyncType.Pong:
					for (int i = 0; i < _state.Length; _state[(_state.Length - 1) - i] = i++) { }
					break;
				case SyncType.Locked:
				case SyncType.Deck:
				case SyncType.Cdeck:
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
