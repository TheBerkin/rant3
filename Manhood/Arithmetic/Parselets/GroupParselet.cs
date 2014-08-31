using Stringes.Tokens;

namespace Manhood.Arithmetic.Parselets
{
    internal class GroupParselet : IPrefixParselet
    {
        public Expression Parse(Parser parser, Token<TokenType> token)
        {
            var e = parser.ParseExpression();
            parser.Take(TokenType.RightParen);
            return e;
        }
    }
}