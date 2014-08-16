namespace Manhood.Arithmetic
{
    internal class PrefixOperatorExpression : Expression
    {
        private readonly Token _token;
        private readonly Expression _right;

        public PrefixOperatorExpression(Token token, Expression right)
        {
            _token = token;
            _right = right;
        }

        public override double Evaluate(Interpreter ii)
        {
            var name = _right as NameExpression;
            switch (_token.Type)
            {
                case TokenType.Minus:
                    return -_right.Evaluate(ii);
                case TokenType.Increment:
                {
                    if (name == null)
                    {
                        throw new ManhoodException("Increment prefix could not find a variable.");
                    }
                    double d = name.Evaluate(ii) + 1;
                    ii.State.Variables.SetVar(name.Name, d);
                    return d;
                }
                case TokenType.Decrement:
                {
                    if (name == null)
                    {
                        throw new ManhoodException("Decrement prefix could not find a variable.");
                    }
                    double d = name.Evaluate(ii) - 1;
                    ii.State.Variables.SetVar(name.Name, d);
                    return d;
                }
                default:
                    throw new ManhoodException("Invalid prefix operator '" + _token + "'.");
            }
        }
    }
}