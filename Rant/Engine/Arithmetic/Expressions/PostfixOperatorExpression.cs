using Rant.Engine.Stringes.Tokens;

namespace Rant.Engine.Arithmetic.Expressions
{
    internal class PostfixOperatorExpression : Expression
    {
        private readonly Token<RMathToken> _token;
        private readonly Expression _left;

        public PostfixOperatorExpression(Token<RMathToken> token, Expression left)
        {
            _token = token;
            _left = left;
        }

        public override double Evaluate(MathParser parser, VM ii)
        {
            var name = _left as NameExpression;
            if (name == null)
            {
                throw new RantException(parser.Source, _token, "Left side of increment/decrement postfix was not a variable.");
            }
            switch (_token.ID)
            {
                case RMathToken.Increment:
                {
                    double d = name.Evaluate(parser, ii);
                    ii.Engine.Variables.SetVar(name.Name, d + 1);
                    return d;
                }
                case RMathToken.Decrement:
                {
                    double d = name.Evaluate(parser, ii);
                    ii.Engine.Variables.SetVar(name.Name, d - 1);
                    return d;
                }
                default:
                    throw new RantException(parser.Source, _token, "Invalid postfix operator '" + _token.Value + "'.");
            }
        }
    }
}