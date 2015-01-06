using Rant.Engine.Arithmetic.Expressions;
using Rant.Stringes.Tokens;

namespace Rant.Engine.Arithmetic.Parselets
{
    internal class NameParselet : IPrefixParselet
    {
        public Expression Parse(MathParser parser, Token<RMathToken> token)
        {
            return new NameExpression(token);
        }
    }
}