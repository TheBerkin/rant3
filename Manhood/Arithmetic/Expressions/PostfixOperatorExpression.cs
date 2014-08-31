using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood.Arithmetic
{
    internal class PostfixOperatorExpression : Expression
    {
        private readonly Token<TokenType> _token;
        private readonly Expression _left;

        public PostfixOperatorExpression(Token<TokenType> token, Expression left)
        {
            _token = token;
            _left = left;
        }

        public override double Evaluate(Source source, Interpreter ii)
        {
            var name = _left as NameExpression;
            if (name == null)
            {
                throw new ManhoodException(source, _token, "Left side of increment/decrement postfix was not a variable.");
            }
            switch (_token.Identifier)
            {
                case TokenType.Increment:
                {
                    double d = name.Evaluate(ii);
                    ii.Engine.Variables.SetVar(name.Name, d + 1);
                    return d;
                }
                case TokenType.Decrement:
                {
                    double d = name.Evaluate(ii);
                    ii.Engine.Variables.SetVar(name.Name, d - 1);
                    return d;
                }
                default:
                    throw new ManhoodException(source, _token, "Invalid postfix operator '" + _token.Value + "'.");
            }
        }
    }
}