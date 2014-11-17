using System;
using Rant.Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal class NumberParselet : IPrefixParselet
    {
        public Expression Parse(Parser parser, Token<RMathToken> token)
        {
            return new NumberExpression(Double.Parse(token.Value));
        }
    }
}