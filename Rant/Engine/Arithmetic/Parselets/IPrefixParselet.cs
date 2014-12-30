using Rant.Engine.Arithmetic.Expressions;
using Rant.Engine.Stringes.Tokens;

namespace Rant.Engine.Arithmetic.Parselets
{
    internal interface IPrefixParselet
    {
        Expression Parse(MathParser parser, Token<RMathToken> token);
    }
}