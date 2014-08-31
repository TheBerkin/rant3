using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood.Arithmetic
{
    internal class PrefixOperatorExpression : Expression
    {
        private readonly Token<TokenType> _token;
        private readonly Expression _right;

        public PrefixOperatorExpression(Token<TokenType> token, Expression right)
        {
            _token = token;
            _right = right;
        }

        public override double Evaluate(Source source, Interpreter ii)
        {
            var name = _right as NameExpression;
            switch (_token.Identifier)
            {
                case TokenType.Minus:
                    return -_right.Evaluate(source, ii);
                case TokenType.Increment:
                {
                    if (name == null)
                    {
                        throw new ManhoodException(source, _token, "Increment prefix could not find a variable.");
                    }
                    double d = name.Evaluate(ii) + 1;
                    ii.Engine.Variables.SetVar(name.Name, d);
                    return d;
                }
                case TokenType.Decrement:
                {
                    if (name == null)
                    {
                        throw new ManhoodException(source, _token, "Decrement prefix could not find a variable.");
                    }
                    double d = name.Evaluate(ii) - 1;
                    ii.Engine.Variables.SetVar(name.Name, d);
                    return d;
                }
                default:
                    throw new ManhoodException(source, _token, "Invalid prefix operator '" + _token + "'.");
            }
        }
    }
}