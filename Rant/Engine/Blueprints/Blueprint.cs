namespace Rant.Blueprints
{
    /// <summary>
    /// Represents an outline for an action that is queued by a state object and executed immediately before or after parsing.
    /// </summary>
    internal abstract class Blueprint
    {
        protected readonly Interpreter I;

        protected Blueprint(Interpreter interpreter)
        {
            I = interpreter;
        }

        public abstract bool Use();
    }
}