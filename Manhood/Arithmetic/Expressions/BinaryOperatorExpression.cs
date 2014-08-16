using System;
using System.Collections.Generic;

namespace Manhood.Arithmetic
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
            if (_token.Type == TokenType.Swap)
            {
                var left = _left as NameExpression;
                var right = _right as NameExpression;
                if (left == null) throw new ManhoodException("Left side of swap operation was not a variable.");
                if (right == null) throw new ManhoodException("Right side of swap operation was not a variable.");
                double temp = left.Evaluate(ii);
                double b = right.Evaluate(ii);
                ii.State.Variables.SetVar(left.Name, b);
                ii.State.Variables.SetVar(right.Name, temp);
                return b;
            }
            Func<Interpreter, NameExpression, Expression, double> assignFunc;
            if (AssignOperations.TryGetValue(_token.Type, out assignFunc))
            {
                var left = _left as NameExpression;
                if (left == null) throw new ManhoodException("Left side of assignment was not a variable.");
                return assignFunc(ii, left, _right);
            }

            Func<double, double, double> func;
            if (!Operations.TryGetValue(_token.Type, out func))
            {
                throw new ManhoodException("Invalid binary operation '" + _token + "'.");
            }
            return func(_left.Evaluate(ii), _right.Evaluate(ii));
        }

        private static readonly Dictionary<TokenType, Func<double, double, double>> Operations;
        private static readonly Dictionary<TokenType, Func<Interpreter, NameExpression, Expression, double>> AssignOperations; 

        static BinaryOperatorExpression()
        {
            Operations = new Dictionary<TokenType, Func<double, double, double>>
            {
                {TokenType.Plus, (a, b) => a + b},
                {TokenType.Minus, (a, b) => a - b},
                {TokenType.Asterisk, (a, b) => a * b},
                {TokenType.Slash, (a, b) => a / b},
                {TokenType.Modulo, (a, b) => a % b},
                {TokenType.Caret, System.Math.Pow}
            };

            AssignOperations = new Dictionary<TokenType, Func<Interpreter, NameExpression, Expression, double>>
            {
                {TokenType.Equals, (ii, a, b) =>
                {
                    double d = b.Evaluate(ii);
                    ii.State.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.AddAssign, (ii, a, b) =>
                {
                    double d = a.Evaluate(ii) + b.Evaluate(ii);
                    ii.State.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.SubAssign, (ii, a, b) =>
                {
                    double d = a.Evaluate(ii) - b.Evaluate(ii);
                    ii.State.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.DivAssign, (ii, a, b) =>
                {
                    double d = a.Evaluate(ii) / b.Evaluate(ii);
                    ii.State.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.MulAssign, (ii, a, b) =>
                {
                    double d = a.Evaluate(ii) * b.Evaluate(ii);
                    ii.State.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.ModAssign, (ii, a, b) =>
                {
                    double d = a.Evaluate(ii) % b.Evaluate(ii);
                    ii.State.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.PowAssign, (ii, a, b) =>
                {
                    double d = System.Math.Pow(a.Evaluate(ii), b.Evaluate(ii));
                    ii.State.Variables.SetVar(a.Name, d);
                    return d;
                }},
            };
        }
    }
}