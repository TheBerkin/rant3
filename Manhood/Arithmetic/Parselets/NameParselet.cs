using Stringes.Tokens;

namespace Manhood.Arithmetic.Parselets
{
    internal class NameParselet : IPrefixParselet
    {
        public Expression Parse(Parser parser, Token<TokenType> token)
        {
            return new NameExpression(token);
        }
    }
}