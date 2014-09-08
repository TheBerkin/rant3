using Stringes.Tokens;

namespace Manhood.Arithmetic.Parselets
{
    internal interface IPrefixParselet
    {
        Expression Parse(Parser parser, Token<MathTokenType> token);
    }
}