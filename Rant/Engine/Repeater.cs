using System.Linq;

using Rant.Blueprints;

namespace Rant
{
    internal class Repeater
    {
        public const int Each = -1;

        private int _index = 0;
        private bool _allowStats = true;
        private readonly int _count;
        private readonly Block _block;
        private readonly BlockAttribs _attribs;

        public Repeater(Block block, BlockAttribs attribs)
        {
            _block = block;
            _count = attribs.Repetitons == Each ? block.Items.Length : attribs.Repetitons;
            _attribs = attribs;
        }

        public bool IsFirst
        {
            get { return _allowStats && _index == 0; }
        }

        public bool IsLast
        {
            get { return _allowStats && _index == _count - 1; }
        }

        // 1-based
        public bool IsOdd
        {
            get { return _allowStats && _index % 2 == 0; }
        }

        // 1-based
        public bool IsEven
        {
            get { return _allowStats && _index % 2 != 0; }
        }

        // 1-based
        public bool Nth(int offset, int interval)
        {
            return _allowStats && (_index - offset + 1) % interval == 0;
        }
        
        public int Count
        {
            get { return _count; }
        }

        public int Index
        {
            get { return _allowStats ? _index : 0; }
        }

        public bool Finished
        {
            get { return _index >= _count; }
        }

        public bool Iterate(Interpreter ii, RepeaterBlueprint bp)
        {
            while (ii.CurrentRepeater != null && ii.CurrentRepeater.Finished)
            {
                ii.PopRepeater();
            }

            if (Finished) return false;

            // Queue the next iteration on the current state
            ii.CurrentState.AddPreBlueprint(bp);

            // Push separator if applicable
            if (!IsLast && _attribs.Separator != null && _attribs.Separator.Any())
            {
                var sepState = Interpreter.State.CreateSub(
                    ii.CurrentState.Reader.Source,
                    _attribs.Separator,
                    ii,
                    ii.CurrentState.Output);

                // Make sure that the repeater is not available to the separator pattern
                sepState.AddPreBlueprint(new DelegateBlueprint(ii, _ =>
                {
                    _allowStats = false;
                    return false;
                }));
                sepState.AddPostBlueprint(new DelegateBlueprint(ii, _ =>
                {
                    _allowStats = true;
                    return false;
                }));

                ii.PushState(sepState);
            }

            Interpreter.State afterState = null;

            // Push postfix if applicable
            if (_attribs.After != null && _attribs.After.Any())
            {
                ii.PushState(afterState = Interpreter.State.CreateSub(
                    ii.CurrentState.Reader.Source,
                    _attribs.After,
                    ii,
                    ii.CurrentState.Output));
            }

            // Push next item
            var itemState = Interpreter.State.CreateSub(ii.CurrentState.Reader.Source,                
                _attribs.Sync != null ? _block.Items[_attribs.Sync.NextItem(_block.Items.Length)].Item2 : _block.Items.PickWeighted(ii.RNG, item => item.Item1).Item2,
                ii,
                ii.CurrentState.Output);

            // Apply the Next() call to the last state in the repeater iteration
            (afterState ?? itemState).AddPostBlueprint(new DelegateBlueprint(ii, _ =>
            {
                Next();
                return false;
            }));

            ii.PushState(itemState);

            // Push prefix if applicable
            if (_attribs.Before != null && _attribs.Before.Any())
            {
                ii.PushState(Interpreter.State.CreateSub(
                    ii.CurrentState.Reader.Source,
                    _attribs.Before,
                    ii,
                    ii.CurrentState.Output));
            }
            
            return true;
        }

        public int Next()
        {
            if (Finished) return -1;
            int i = _index;
            _index++;
            return i;
        }

        public void Finish()
        {
            _index = _count;
        }
    }
}