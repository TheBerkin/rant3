using System;

namespace Manhood
{
    internal class ValueExpression : Expression
    {
        private readonly string _name;

        public ValueExpression(string name)
        {
            _name = name;
        }

        public override double Evaluate(Interpreter ii)
        {
            double n;
            if (Double.TryParse(_name, out n)) return n;
            var d = ii.State.Variables.GetVar(_name);
            if (d == null) throw new ManhoodException("Tried to access undefined variable '" + _name + "'.");
            return d.Value;
        }
    }
}