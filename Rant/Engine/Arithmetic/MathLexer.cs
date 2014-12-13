using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Rant.Stringes;
using Rant.Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal sealed partial class MathLexer : IEnumerable<Token<RMathToken>>
    {
        public static LexerRules<RMathToken> Rules;

        private readonly StringeReader _reader;

        static MathLexer()
        {
            Rules = new LexerRules<RMathToken>
            {
                {"+", RMathToken.Plus},
                {"-", RMathToken.Minus},
                {"*", RMathToken.Asterisk},
                {"/", RMathToken.Slash},
                {"^", RMathToken.Caret},
                {"(", RMathToken.LeftParen},
                {")", RMathToken.RightParen},
                {"++", RMathToken.Increment},
                {"--", RMathToken.Decrement},
                {"%", RMathToken.Modulo},
                {"=", RMathToken.Equals},
                {"$=", RMathToken.Swap},
                {"+=", RMathToken.AddAssign},
                {"-=", RMathToken.SubAssign},
                {"*=", RMathToken.MulAssign},
                {"/=", RMathToken.DivAssign},
                {"%=", RMathToken.ModAssign},
                {"^=", RMathToken.PowAssign},
                {"|", RMathToken.Pipe},
                {new Regex(@"(\d+(\.\d+)?|\.\d+)"), RMathToken.Number},
                {new Regex(@"[a-zA-Z_][a-zA-Z0-9_]*"), RMathToken.Name}
            };
            Rules.AddEndToken(RMathToken.End);
        }

        public MathLexer(Stringe input)
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