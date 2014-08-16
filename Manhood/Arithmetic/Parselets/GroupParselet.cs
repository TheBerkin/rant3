namespace Manhood.Arithmetic.Parselets
{
    internal class GroupParselet : IPrefixParselet
    {
        public Expression Parse(Parser parser, Token token)
        {
            var e = parser.ParseExpression();
            parser.Take(TokenType.RightParen);
            return e;
        }
    }
}