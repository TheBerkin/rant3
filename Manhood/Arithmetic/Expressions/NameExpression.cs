namespace Manhood.Arithmetic
{
    internal class NameExpression : Expression
    {
        private readonly string _name;

        public NameExpression(Token token)
        {
            _name = token.Text;
        }

        public string Name
        {
            get { return _name; }
        }

        public override double Evaluate(Interpreter ii)
        {
            var d = ii.State.Variables.GetVar(_name);
            if (d == null) throw new ManhoodException("Tried to access undefined variable '" + _name + "'.");
            return d.Value;
        }
    }
}