using System.Collections;
using System.Collections.Generic;

using Manhood.Blueprints;
using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood
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
        
        public int Count
        {
            get { return _count; }
        }

        public bool Finished
        {
            get { return _index >= _count; }
        }

        public bool Iterate(Interpreter ii, RepeaterBlueprint bp)
        {
            if (Finished) return false;
            Next();
            ii.CurrentState.AddBlueprint(bp);

            // Push separator if applicable
            if (!Finished && _attribs.Separator != null)
            {
                ii.PushState(Interpreter.State.CreateDerivedDistinct(
                    ii.CurrentState.Reader.Source,
                    _attribs.Separator,
                    ii,
                    ii.CurrentState.Output));
            }

            // Push postfix if applicable
            if (_attribs.After != null)
            {
                ii.PushState(Interpreter.State.CreateDerivedDistinct(
                    ii.CurrentState.Reader.Source,
                    _attribs.After,
                    ii,
                    ii.CurrentState.Output));
            }

            // Push next item
            ii.PushState(Interpreter.State.CreateDerivedDistinct(ii.CurrentState.Reader.Source, _items[ii.RNG.Next(_items.Length)], ii, ii.CurrentState.Output));

            // Push prefix if applicable
            if (_attribs.After != null)
            {
                ii.PushState(Interpreter.State.CreateDerivedDistinct(
                    ii.CurrentState.Reader.Source,
                    _attribs.Before,
                    ii,
                    ii.CurrentState.Output));
            }
            
            return true;
        }

        private int Next()
        {
            if (Finished) return -1;
            int i = _index;
            _index++;
            return i;
        }
    }
}