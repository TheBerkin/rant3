using System;
using System.Collections.Generic;

using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood.Arithmetic
{
    internal class BinaryOperatorExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly Token<TokenType> _token;

        public BinaryOperatorExpression(Expression left, Expression right, Token<TokenType> token)
        {
            _left = left;
            _right = right;
            _token = token;
        }

        public override double Evaluate(Source source, Interpreter ii)
        {
            if (_token.Identifier == TokenType.Swap)
            {
                var left = _left as NameExpression;
                var right = _right as NameExpression;
                if (left == null) throw new ManhoodException(source, _token, "Left side of swap operation was not a variable.");
                if (right == null) throw new ManhoodException(source, _token, "Right side of swap operation was not a variable.");
                double temp = left.Evaluate(source, ii);
                double b = right.Evaluate(source, ii);
                ii.Engine.Variables.SetVar(left.Name, b);
                ii.Engine.Variables.SetVar(right.Name, temp);
                return b;
            }
            Func<Source, Interpreter, NameExpression, Expression, double> assignFunc;
            if (AssignOperations.TryGetValue(_token.Identifier, out assignFunc))
            {
                var left = _left as NameExpression;
                if (left == null) throw new ManhoodException(source, _token, "Left side of assignment was not a variable.");
                return assignFunc(source, ii, left, _right);
            }

            Func<double, double, double> func;
            if (!Operations.TryGetValue(_token.Identifier, out func))
            {
                throw new ManhoodException(source, _token, "Invalid binary operation '" + _token + "'.");
            }
            return func(_left.Evaluate(source, ii), _right.Evaluate(source, ii));
        }

        private static readonly Dictionary<TokenType, Func<double, double, double>> Operations;
        private static readonly Dictionary<TokenType, Func<Source, Interpreter, NameExpression, Expression, double>> AssignOperations; 

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

            AssignOperations = new Dictionary<TokenType, Func<Source, Interpreter, NameExpression, Expression, double>>
            {
                {TokenType.Equals, (s, ii, a, b) =>
                {
                    double d = b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.AddAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) + b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.SubAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) - b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.DivAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) / b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.MulAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) * b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.ModAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) % b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {TokenType.PowAssign, (s, ii, a, b) =>
                {
                    double d = Math.Pow(a.Evaluate(s, ii), b.Evaluate(s, ii));
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
            };
        }
    }
}