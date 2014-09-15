using Rant.Compiler;

using Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal class PrefixOperatorExpression : Expression
    {
        private readonly Token<MathTokenType> _token;
        private readonly Expression _right;

        public PrefixOperatorExpression(Token<MathTokenType> token, Expression right)
        {
            _token = token;
            _right = right;
        }

        public override double Evaluate(Parser parser, Interpreter ii)
        {
            var name = _right as NameExpression;
            switch (_token.Identifier)
            {
                case MathTokenType.Minus:
                    return -_right.Evaluate(parser, ii);
                case MathTokenType.Increment:
                {
                    if (name == null)
                    {
                        throw new RantException(parser.Source, _token, "Increment prefix could not find a variable.");
                    }
                    double d = name.Evaluate(parser, ii) + 1;
                    ii.Engine.Variables.SetVar(name.Name, d);
                    return d;
                }
                case MathTokenType.Decrement:
                {
                    if (name == null)
                    {
                        throw new RantException(parser.Source, _token, "Decrement prefix could not find a variable.");
                    }
                    double d = name.Evaluate(parser, ii) - 1;
                    ii.Engine.Variables.SetVar(name.Name, d);
                    return d;
                }
                default:
                    throw new RantException(parser.Source, _token, "Invalid prefix operator '" + _token + "'.");
            }
        }
    }
}