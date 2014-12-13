using Rant.Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal interface IPrefixParselet
    {
        Expression Parse(MathParser parser, Token<RMathToken> token);
    }
}