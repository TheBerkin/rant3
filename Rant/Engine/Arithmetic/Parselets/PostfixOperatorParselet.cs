using Rant.Engine.Arithmetic.Expressions;
using Rant.Engine.Stringes.Tokens;

namespace Rant.Engine.Arithmetic.Parselets
{
    internal class PostfixOperatorParselet : IInfixParselet
    {
        private readonly int _precedence;

        public PostfixOperatorParselet(int precedence)
        {
            _precedence = precedence;
        }

        public int Precedence
        {
            get { return _precedence; }
        }

        public Expression Parse(MathParser parser, Expression left, Token<RMathToken> token)
        {
            return new PostfixOperatorExpression(token, left);
        }
    }
}