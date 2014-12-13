using Rant.Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal class PrefixOperatorExpression : Expression
    {
        private readonly Token<RMathToken> _token;
        private readonly Expression _right;

        public PrefixOperatorExpression(Token<RMathToken> token, Expression right)
        {
            _token = token;
            _right = right;
        }

        public override double Evaluate(MathParser parser, VM ii)
        {
            var name = _right as NameExpression;
            switch (_token.ID)
            {
                case RMathToken.Minus:
                    return -_right.Evaluate(parser, ii);
                case RMathToken.Increment:
                {
                    if (name == null)
                    {
                        throw new RantException(parser.Source, _token, "Increment prefix could not find a variable.");
                    }
                    double d = name.Evaluate(parser, ii) + 1;
                    ii.Engine.Variables.SetVar(name.Name, d);
                    return d;
                }
                case RMathToken.Decrement:
                {
                    if (name == null)
                    {
                        throw new RantException(parser.Source, _token, "Decrement prefix could not find a variable.");
                    }
                    double d = name.Evaluate(parser, ii) - 1;
                    ii.Engine.Variables.SetVar(name.Name, d);
                    return d;
                }
                case RMathToken.Slash:
                {
                    return 1.0 / _right.Evaluate(parser, ii);
                }
                default:
                    throw new RantException(parser.Source, _token, "Invalid prefix operator '" + _token + "'.");
            }
        }
    }
}