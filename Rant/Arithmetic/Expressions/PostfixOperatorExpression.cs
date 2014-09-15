using Rant.Compiler;

using Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal class PostfixOperatorExpression : Expression
    {
        private readonly Token<MathTokenType> _token;
        private readonly Expression _left;

        public PostfixOperatorExpression(Token<MathTokenType> token, Expression left)
        {
            _token = token;
            _left = left;
        }

        public override double Evaluate(Parser parser, Interpreter ii)
        {
            var name = _left as NameExpression;
            if (name == null)
            {
                throw new RantException(parser.Source, _token, "Left side of increment/decrement postfix was not a variable.");
            }
            switch (_token.Identifier)
            {
                case MathTokenType.Increment:
                {
                    double d = name.Evaluate(parser, ii);
                    ii.Engine.Variables.SetVar(name.Name, d + 1);
                    return d;
                }
                case MathTokenType.Decrement:
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