using System;
using System.Collections.Generic;

using Rant.Engine.Stringes.Tokens;

namespace Rant.Engine.Arithmetic.Expressions
{
    internal class BinaryOperatorExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly Token<RMathToken> _token;

        public BinaryOperatorExpression(Expression left, Expression right, Token<RMathToken> token)
        {
            _left = left;
            _right = right;
            _token = token;
        }

        public override double Evaluate(MathParser parser, VM ii)
        {
            if (_token.ID == RMathToken.Swap)
            {
                var left = _left as NameExpression;
                var right = _right as NameExpression;
                if (left == null) throw new RantException(parser.Source, _token, "Left side of swap operation was not a variable.");
                if (right == null) throw new RantException(parser.Source, _token, "Right side of swap operation was not a variable.");
                double temp = left.Evaluate(parser, ii);
                double b = right.Evaluate(parser, ii);
                ii.Engine.Variables.SetVar(left.Name, b);
                ii.Engine.Variables.SetVar(right.Name, temp);
                return b;
            }
            Func<MathParser, VM, NameExpression, Expression, double> assignFunc;
            if (AssignOperations.TryGetValue(_token.ID, out assignFunc))
            {
                var left = _left as NameExpression;
                if (left == null) throw new RantException(parser.Source, _token, "Left side of assignment was not a variable.");
                return assignFunc(parser, ii, left, _right);
            }

            Func<double, double, double> func;
            if (!Operations.TryGetValue(_token.ID, out func))
            {
                throw new RantException(parser.Source, _token, "Invalid binary operation '" + _token + "'.");
            }
            return func(_left.Evaluate(parser, ii), _right.Evaluate(parser, ii));
        }

        private static readonly Dictionary<RMathToken, Func<double, double, double>> Operations;
        private static readonly Dictionary<RMathToken, Func<MathParser, VM, NameExpression, Expression, double>> AssignOperations; 

        static BinaryOperatorExpression()
        {
            Operations = new Dictionary<RMathToken, Func<double, double, double>>
            {
                {RMathToken.Plus, (a, b) => a + b},
                {RMathToken.Minus, (a, b) => a - b},
                {RMathToken.Asterisk, (a, b) => a * b},
                {RMathToken.Slash, (a, b) => a / b},
                {RMathToken.Modulo, (a, b) => a % b},
                {RMathToken.Caret, Math.Pow},
                {RMathToken.Root, (a, b) => Math.Pow(a, 1.0 / b)}
            };

            AssignOperations = new Dictionary<RMathToken, Func<MathParser, VM, NameExpression, Expression, double>>
            {
                {RMathToken.Equals, (s, ii, a, b) =>
                {
                    double d = b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {RMathToken.AddAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) + b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {RMathToken.SubAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) - b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {RMathToken.DivAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) / b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {RMathToken.MulAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) * b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {RMathToken.ModAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) % b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {RMathToken.PowAssign, (s, ii, a, b) =>
                {
                    double d = Math.Pow(a.Evaluate(s, ii), b.Evaluate(s, ii));
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {RMathToken.RootAssign, (s, ii, a, b) =>
                {
                    double d = Math.Pow(a.Evaluate(s, ii), 1.0 / b.Evaluate(s, ii));
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }}
            };
        }
    }
}