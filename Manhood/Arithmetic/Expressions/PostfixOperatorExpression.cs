namespace Manhood.Arithmetic
{
    internal class PostfixOperatorExpression : Expression
    {
        private readonly Token _token;
        private readonly Expression _left;

        public PostfixOperatorExpression(Token token, Expression left)
        {
            _token = token;
            _left = left;
        }

        public override double Evaluate(Interpreter ii)
        {
            var name = _left as NameExpression;
            if (name == null)
            {
                throw new ManhoodException("Left side of increment/decrement postfix was not a variable.");
            }
            switch (_token.Type)
            {
                case TokenType.Increment:
                {
                    double d = name.Evaluate(ii);
                    ii.State.Variables.SetVar(name.Name, d + 1);
                    return d;
                }
                case TokenType.Decrement:
                {
                    double d = name.Evaluate(ii);
                    ii.State.Variables.SetVar(name.Name, d - 1);
                    return d;
                }
                default:
                    throw new ManhoodException("Invalid postfix operator '" + _token.Text + "'.");
            }
        }
    }
}