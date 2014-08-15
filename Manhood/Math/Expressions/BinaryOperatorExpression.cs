using System;
using System.Collections.Generic;

namespace Manhood
{
    internal class BinaryOperatorExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly Token _token;

        public BinaryOperatorExpression(Expression left, Expression right, Token token)
        {
            _left = left;
            _right = right;
            _token = token;
        }

        public override double Evaluate(Interpreter ii)
        {
            if (_token.Type == TokenType.Equals)
            {
                var variable = _left as NameExpression;
                var value = _right.Evaluate(ii);
                if (variable == null)
                    throw new ManhoodException("Tried to assign a value to something that wasn't a variable.");
                ii.State.Variables.SetVar(variable.Name, value);
                return value;
            }
            Func<double, double, double> func;
            if (!Operations.TryGetValue(_token.Type, out func))
            {
                throw new ManhoodException("Invalid binary operation " + _token);
            }
            return func(_left.Evaluate(ii), _right.Evaluate(ii));
        }

        private static readonly Dictionary<TokenType, Func<double, double, double>> Operations;

        static BinaryOperatorExpression()
        {
            Operations = new Dictionary<TokenType, Func<double, double, double>>()
            {
                {TokenType.Plus, (a, b) => a + b},
                {TokenType.Minus, (a, b) => a - b},
                {TokenType.Asterisk, (a, b) => a * b},
                {TokenType.Slash, (a, b) => a / b},
                {TokenType.Modulo, (a, b) => a % b},
                {TokenType.Caret, Math.Pow}
            };
        }
    }
}