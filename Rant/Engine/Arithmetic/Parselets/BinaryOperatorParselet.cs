using Rant.Engine.Arithmetic.Expressions;
using Rant.Stringes.Tokens;

namespace Rant.Engine.Arithmetic.Parselets
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

        public Expression Parse(MathParser parser, Expression left, Token<RMathToken> token)
        {
            var right = parser.ParseExpression(Precedence - (_right ? 1 : 0));
            return new BinaryOperatorExpression(left, right, token);
        }
    }
}