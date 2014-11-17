using Rant.Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal class NameParselet : IPrefixParselet
    {
        public Expression Parse(Parser parser, Token<RMathToken> token)
        {
            return new NameExpression(token);
        }
    }
}