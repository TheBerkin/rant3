namespace Rant.Engine.Arithmetic.Expressions
{
    internal abstract class Expression
    {
        public abstract double Evaluate(MathParser parser, VM ii);
    }
}