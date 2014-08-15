namespace Manhood.Parselets
{
    internal class NameParselet : IPrefixParselet
    {
        public Expression Parse(Parser parser, Token token)
        {
            return new NameExpression(token);
        }
    }
}