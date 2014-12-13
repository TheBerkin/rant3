using Rant.Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal class GroupParselet : IPrefixParselet
    {
        public Expression Parse(MathParser parser, Token<RMathToken> token)
        {
            var e = parser.ParseExpression();
            parser.Take(RMathToken.RightParen);
            return e;
        }
    }
}