namespace Rant.Arithmetic
{
    internal abstract class Expression
    {
        public abstract double Evaluate(MathParser parser, VM ii);
    }
}