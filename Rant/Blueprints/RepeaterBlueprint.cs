namespace Rant.Blueprints
{
    internal sealed class RepeaterBlueprint : Blueprint
    {
        private readonly Repeater _repeater;

        public RepeaterBlueprint(Interpreter interpreter, Repeater repeater)
            : base(interpreter)
        {
            _repeater = repeater;
        }

        public override bool Use()
        {
            return _repeater.Iterate(I, this);
        }
    }
}