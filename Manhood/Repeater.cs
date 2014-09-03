using System.Collections;
using System.Collections.Generic;

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
        private readonly IEnumerable<Token<TokenType>> _separator;

        public Repeater(IEnumerable<Token<TokenType>>[] items, Interpreter.BlockAttribs attribs)
        {
            _index = 0;
            _items = items;
            _count = attribs.Repetitons == Each ? items.Length : attribs.Repetitons;
            _separator = attribs.Separator;
        }
        
        public int Count
        {
            get { return _count; }
        }

        public bool Finished
        {
            get { return _index >= _count; }
        }

        public bool Iterate(Interpreter ii, Interpreter.RepeaterBlueprint bp)
        {
            if (Finished) return false;
            Next();
            ii.CurrentState.AddBlueprint(bp);

            // Push separator if applicable
            if (!Finished && _separator != null)
            {
                ii.PushState(Interpreter.State.CreateDerivedDistinct(
                    ii.CurrentState.Reader.Source,
                    _separator,
                    ii,
                    ii.CurrentState.Output));
            }

            // Push next item
            ii.PushState(Interpreter.State.CreateDerivedDistinct(ii.CurrentState.Reader.Source, _items[ii.RNG.Next(_items.Length)], ii, ii.CurrentState.Output));
            return true;
        }

        public IEnumerable<Token<TokenType>> Separator
        {
            get { return _separator; }
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