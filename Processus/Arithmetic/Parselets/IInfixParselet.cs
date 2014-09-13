using Stringes.Tokens;

namespace Processus.Arithmetic.Parselets
{
    internal interface IInfixParselet
    {
        int Precedence { get; }

        Expression Parse(Parser parser, Expression left, Token<MathTokenType> token);
    }
}