using System;

namespace Manhood
{
    internal class BinaryOperationExpression : Expression
    {
        private readonly Expression leftExpression;
        private readonly TokenType operation;
        private readonly Expression rightExpression;

        public BinaryOperationExpression(Expression left, TokenType op, Expression right)
        {
            leftExpression = left;
            rightExpression = right;
            operation = op;
        }

        public override double Evaluate(Interpreter ii)
        {
            throw new NotImplementedException();
        }
    }
}