using Stringes.Tokens;

namespace Processus.Arithmetic.Parselets
{
    internal interface IPrefixParselet
    {
        Expression Parse(Parser parser, Token<MathTokenType> token);
    }
}