using Rant.Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal class PrefixOperatorParselet : IPrefixParselet
    {
        private readonly int _precedence;

        public PrefixOperatorParselet(int precedence)
        {
            _precedence = precedence;
        }

        public Expression Parse(MathParser parser, Token<RMathToken> token)
        {
            var right = parser.ParseExpression(_precedence);
            return new PrefixOperatorExpression(token, right);
        }
    }
}