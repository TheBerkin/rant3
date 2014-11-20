using System.Collections;
using System.Collections.Generic;
using Rant.Stringes;
using Rant.Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal sealed partial class Lexer : IEnumerable<Token<RMathToken>>
    {
        private readonly StringeReader _reader;

        public Lexer(Stringe input)
        {
            _reader = new StringeReader(input);
        }

        public IEnumerator<Token<RMathToken>> GetEnumerator()
        {
            while (!_reader.EndOfStringe)
            {
                _reader.SkipWhiteSpace();
                yield return _reader.ReadToken(Rules);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}