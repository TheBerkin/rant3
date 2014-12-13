using System;

namespace Rant.Arithmetic.Expressions
{
    internal class AbsExpression : Expression
    {
        private readonly Expression _expr;

        public AbsExpression(Expression expression)
        {
            _expr = expression;
        }

        public override double Evaluate(MathParser parser, VM ii)
        {
            return Math.Abs(_expr.Evaluate(parser, ii));
        }
    }
}