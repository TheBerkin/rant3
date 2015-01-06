using Rant.Engine.Arithmetic.Expressions;
using Rant.Stringes.Tokens;

namespace Rant.Engine.Arithmetic.Parselets
{
    internal interface IInfixParselet
    {
        int Precedence { get; }

        Expression Parse(MathParser parser, Expression left, Token<RMathToken> token);
    }
}