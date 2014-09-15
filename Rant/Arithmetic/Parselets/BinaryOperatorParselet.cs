using Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal class BinaryOperatorParselet : IInfixParselet
    {
        private readonly int _precedence;
        private readonly bool _right;

        public BinaryOperatorParselet(int precedence, bool right)
        {
            _precedence = precedence;
            _right = right;
        }

        public int Precedence
        {
            get { return _precedence; }
        }

        public Expression Parse(Parser parser, Expression left, Token<MathTokenType> token)
        {
            var right = parser.ParseExpression(Precedence - (_right ? 1 : 0));
            return new BinaryOperatorExpression(left, right, token);
        }
    }
}