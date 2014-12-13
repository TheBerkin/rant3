namespace Rant.Arithmetic
{
    internal class NumberExpression : Expression
    {
        private readonly double _number;

        public NumberExpression(double number)
        {
            _number = number;
        }

        public double Value
        {
            get { return _number; }
        }

        public override double Evaluate(MathParser parser, VM ii)
        {
            return _number;
        }
    }
}