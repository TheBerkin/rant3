namespace Manhood.Parselets
{
    internal class PrefixOperatorParselet : IPrefixParselet
    {
        private readonly int _precedence;

        public PrefixOperatorParselet(int precedence)
        {
            _precedence = precedence;
        }

        public Expression Parse(Parser parser, Token token)
        {
            var right = parser.ParseExpression(_precedence);
            return new PrefixOperatorExpression(token, right);
        }
    }
}