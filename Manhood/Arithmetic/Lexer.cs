using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Stringes;
using Stringes.Tokens;

namespace Manhood.Arithmetic
{
    internal sealed partial class Lexer : IEnumerable<Token<TokenType>>
    {
        private readonly StringeReader _reader;

        public Lexer(Stringe input)
        {
            _reader = new StringeReader(input);
        }

        public IEnumerator<Token<TokenType>> GetEnumerator()
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