using Rant.Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal class NameExpression : Expression
    {
        private readonly Token<RMathToken> _name;

        public NameExpression(Token<RMathToken> token)
        {
            _name = token;
        }

        public string Name
        {
            get { return _name.Value; }
        }

        public override double Evaluate(MathParser parser, VM ii)
        {
            var d = ii.Engine.Variables.GetVar(_name.Value);
            if (d == null) throw new RantException(parser.Source, _name, "Tried to access undefined variable '" + _name.Value + "'.");
            return d.Value;
        }
    }
}