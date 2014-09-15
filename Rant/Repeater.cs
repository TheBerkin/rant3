using System.Collections.Generic;
using System.Linq;

using Rant.Blueprints;
using Rant.Compiler;

using Stringes.Tokens;

namespace Rant
{
    internal class Repeater
    {
        public const int Each = -1;

        private int _index;
        private readonly int _count;
        private readonly IEnumerable<Token<TokenType>>[] _items;
        private readonly BlockAttribs _attribs;

        public Repeater(IEnumerable<Token<TokenType>>[] items, BlockAttribs attribs)
        {
            _index = 0;
            _items = items;
            _count = attribs.Repetitons == Each ? items.Length : attribs.Repetitons;
            _attribs = attribs;
        }

        public bool IsFirst
        {
            get { return _index == 0; }
        }

        public bool IsLast
        {
            get { return _index == _count - 1; }
        }

        // 1-based
        public bool IsOdd
        {
            get { return _index % 2 == 0; }
        }

        // 1-based
        public bool IsEven
        {
            get { return _index % 2 != 0; }
        }

        // 1-based
        public bool Nth(int offset, int interval)
        {
            return ((_index + 1) - offset) % interval == 0;
        }
        
        public int Count
        {
            get { return _count; }
        }

        public int Index
        {
            get { return _index; }
        }

        public bool Finished
        {
            get { return _index >= _count; }
        }

        public bool Iterate(Interpreter ii, RepeaterBlueprint bp)
        {
            if (Finished)
            {
                ii.PopRepeater();
                return false;
            }

            // Queue the next iteration on the current state
            ii.CurrentState.AddPreBlueprint(bp);

            // Push separator if applicable
            if (!IsLast && _attribs.Separator != null && _attribs.Separator.Any())
            {
                var sepState = Interpreter.State.CreateDerivedDistinct(
                    ii.CurrentState.Reader.Source,
                    _attribs.Separator,
                    ii,
                    ii.CurrentState.Output);

                // Make sure that the repeater is not available to the separator pattern
                sepState.AddPreBlueprint(new RepeaterStackBlueprint(ii, this, RepeaterStackAction.Pop));
                sepState.AddPostBlueprint(new RepeaterStackBlueprint(ii, this, RepeaterStackAction.Push));

                ii.PushState(sepState);
            }

            // Push postfix if applicable
            if (_attribs.After != null && _attribs.After.Any())
            {
                ii.PushState(Interpreter.State.CreateDerivedDistinct(
                    ii.CurrentState.Reader.Source,
                    _attribs.After,
                    ii,
                    ii.CurrentState.Output));
            }

            // Push next item
            var itemState = Interpreter.State.CreateDerivedDistinct(ii.CurrentState.Reader.Source,
                _items[_attribs.Sync != null ? _attribs.Sync.NextItem(_items.Length) : ii.RNG.Next(_items.Length)],
                ii,
                ii.CurrentState.Output);

            // Add a blueprint that iterates the repeater just before reading the item. This makes sure that tags like [first] can run before this happens.
            itemState.AddPostBlueprint(new FunctionBlueprint(ii, _ =>
            {
                Next();
                return false;
            }));

            ii.PushState(itemState);

            // Push prefix if applicable
            if (_attribs.Before != null && _attribs.Before.Any())
            {
                ii.PushState(Interpreter.State.CreateDerivedDistinct(
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
    }
}