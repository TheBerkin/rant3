using Rant.Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal interface IInfixParselet
    {
        int Precedence { get; }

        Expression Parse(MathParser parser, Expression left, Token<RMathToken> token);
    }
}