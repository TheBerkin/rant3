namespace Manhood.Arithmetic.Parselets
{
    internal interface IPrefixParselet
    {
        Expression Parse(Parser parser, Token token);
    }
}