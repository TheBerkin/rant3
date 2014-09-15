using Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal class GroupParselet : IPrefixParselet
    {
        public Expression Parse(Parser parser, Token<MathTokenType> token)
        {
            var e = parser.ParseExpression();
            parser.Take(MathTokenType.RightParen);
            return e;
        }
    }
}