using Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal interface IPrefixParselet
    {
        Expression Parse(Parser parser, Token<MathTokenType> token);
    }
}