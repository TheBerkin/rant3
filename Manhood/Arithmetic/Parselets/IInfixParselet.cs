namespace Manhood.Arithmetic.Parselets
{
    internal interface IInfixParselet
    {
        int Precedence { get; }

        Expression Parse(Parser parser, Expression left, Token token);
    }
}