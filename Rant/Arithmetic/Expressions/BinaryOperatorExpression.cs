using System;
using System.Collections.Generic;

using Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal class BinaryOperatorExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly Token<MathTokenType> _token;

        public BinaryOperatorExpression(Expression left, Expression right, Token<MathTokenType> token)
        {
            _left = left;
            _right = right;
            _token = token;
        }

        public override double Evaluate(Parser parser, Interpreter ii)
        {
            if (_token.Identifier == MathTokenType.Swap)
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
            Func<Parser, Interpreter, NameExpression, Expression, double> assignFunc;
            if (AssignOperations.TryGetValue(_token.Identifier, out assignFunc))
            {
                var left = _left as NameExpression;
                if (left == null) throw new RantException(parser.Source, _token, "Left side of assignment was not a variable.");
                return assignFunc(parser, ii, left, _right);
            }

            Func<double, double, double> func;
            if (!Operations.TryGetValue(_token.Identifier, out func))
            {
                throw new RantException(parser.Source, _token, "Invalid binary operation '" + _token + "'.");
            }
            return func(_left.Evaluate(parser, ii), _right.Evaluate(parser, ii));
        }

        private static readonly Dictionary<MathTokenType, Func<double, double, double>> Operations;
        private static readonly Dictionary<MathTokenType, Func<Parser, Interpreter, NameExpression, Expression, double>> AssignOperations; 

        static BinaryOperatorExpression()
        {
            Operations = new Dictionary<MathTokenType, Func<double, double, double>>
            {
                {MathTokenType.Plus, (a, b) => a + b},
                {MathTokenType.Minus, (a, b) => a - b},
                {MathTokenType.Asterisk, (a, b) => a * b},
                {MathTokenType.Slash, (a, b) => a / b},
                {MathTokenType.Modulo, (a, b) => a % b},
                {MathTokenType.Caret, System.Math.Pow}
            };

            AssignOperations = new Dictionary<MathTokenType, Func<Parser, Interpreter, NameExpression, Expression, double>>
            {
                {MathTokenType.Equals, (s, ii, a, b) =>
                {
                    double d = b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {MathTokenType.AddAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) + b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {MathTokenType.SubAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) - b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {MathTokenType.DivAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) / b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {MathTokenType.MulAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) * b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {MathTokenType.ModAssign, (s, ii, a, b) =>
                {
                    double d = a.Evaluate(s, ii) % b.Evaluate(s, ii);
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
                {MathTokenType.PowAssign, (s, ii, a, b) =>
                {
                    double d = Math.Pow(a.Evaluate(s, ii), b.Evaluate(s, ii));
                    ii.Engine.Variables.SetVar(a.Name, d);
                    return d;
                }},
            };
        }
    }
}