using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Manhood
{
    internal class State
    {
        public Capitalization CurrentFormat = Capitalization.None;

        private readonly Dictionary<string, Synchronizer> _selectors = new Dictionary<string, Synchronizer>();
        private readonly SubStore _subStore;
        private readonly HashSet<string> _pinQueue = new HashSet<string>(); 
        private readonly RNG _rng;
        private readonly Stack<SubArgs> _argStack;
        private readonly Stack<Match> _matchStack;
        private readonly Stack<Repeater> _repeaters;
        private readonly Stack<int> _pickers; 

        private Synchronizer _activeSelector;

        public State(SubStore subStore, long seed)
        {
            _rng = new RNG(seed);
            _activeSelector = null;
            _argStack = new Stack<SubArgs>();
            _matchStack = new Stack<Match>();
            _repeaters = new Stack<Repeater>();
            _pickers = new Stack<int>();
            _subStore = subStore;
        }

        public void PushPicker(int max)
        {
            _pickers.Push(_rng.Next(max) + 1);
        }

        public bool PickerActive
        {
            get { return _pickers.Any(); }
        }

        public bool TestPickerThreshold(int number)
        {
            if (!_pickers.Any()) return false;
            return _pickers.Peek() > number;
        }

        public void PopPicker()
        {
            _pickers.Pop();
        }

        public void PushRepeater(Repeater repStats)
        {
            _repeaters.Push(repStats);
        }

        public Repeater PopRepeater()
        {
            return _repeaters.Pop();
        }

        public Repeater CurrentRepeater
        {
            get { return _repeaters.Count == 0 ? null : _repeaters.Peek(); }
        }

        public void PushArgs(SubArgs args)
        {
            _argStack.Push(args);
        }

        public void PopArgs()
        {
            _argStack.Pop();
        }

        public SubArgs CurrentArgs
        {
            get
            {
                return _argStack.Count == 0 ? null : _argStack.Peek();
            }
        }

        public void PushMatch(Match result)
        {
            _matchStack.Push(result);
        }

        public void PopMatch()
        {
            _matchStack.Pop();
        }

        public Match CurrentMatch
        {
            get { return _matchStack.Count == 0 ? null : _matchStack.Peek(); }
        }

        public RNG RNG
        {
            get { return _rng; }
        }

        public SubStore Subroutines
        {
            get { return _subStore; }
        }

        public bool SetPinState(string id, bool pinned)
        {
            if (!Util.ValidateName(id)) throw new FormatException("Invalid selector ID '" + id + "'.");
            Synchronizer info;
            if (!_selectors.TryGetValue(id, out info))
            {
                if (pinned) _pinQueue.Add(id);
                return true;
            }
            info.Pinned = pinned;
            return true;
        }

        public bool Step(string id)
        {
            if (!Util.ValidateName(id)) throw new FormatException("Invalid selector ID '" + id + "'.");
            Synchronizer info;
            if (!_selectors.TryGetValue(id, out info)) return false;
            info.Step(true);
            return true;
        }

        public void Sync(string id, SelectorType type)
        {
            if (!Util.ValidateName(id)) throw new FormatException("Invalid selector ID '" + id + "'.");
            Synchronizer info;
            if (!_selectors.TryGetValue(id, out info))
            {
                info = new Synchronizer(type, RNG.GetRaw(id.Hash(), _rng.NextRaw()));
                if (_pinQueue.Remove(id))
                {
                    info.Pinned = true;
                }
                _selectors[id] = info;
            }
            else
            {
                info.Type = type;
            }

            _activeSelector = info;
        }

        public void Reseed(string id, string seed)
        {
            if (!Util.ValidateName(id)) throw new FormatException("Invalid selector ID '" + id + "'.");
            Synchronizer info;
            if (_selectors.TryGetValue(id, out info))
            {
                _selectors[id] = new Synchronizer(info.Type, _rng.NextRaw());
            }
        }

        public void Reset(string id)
        {
            if (!Util.ValidateName(id)) throw new FormatException("Invalid selector ID '" + id + "'.");
            Synchronizer info;
            if (_selectors.TryGetValue(id, out info))
            {
                info.Reset();
            }
        }

        public Synchronizer PopSynchronizer()
        {
            var si = _activeSelector;
            _activeSelector = null;
            return si;
        }

        public int SelectBlockItem(int count, Synchronizer selector = null)
        {
            return selector == null ? _rng.Next(count) : selector.NextItem(count);
        }

        public void Desync()
        {
            _activeSelector = null;
        }
    }
}