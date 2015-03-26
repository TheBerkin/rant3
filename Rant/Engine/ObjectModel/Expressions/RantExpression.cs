namespace Rant.Engine.ObjectModel.Expressions
{
    internal abstract class RantExpression
    {
        public abstract RantObject Evaluate(VM vm);
    }
}