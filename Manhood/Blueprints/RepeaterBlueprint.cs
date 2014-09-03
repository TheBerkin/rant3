namespace Manhood.Blueprints
{
    internal sealed class RepeaterBlueprint : Blueprint
    {
        private readonly Repeater _repeater;

        public RepeaterBlueprint(Interpreter interpreter, Repeater repeater)
            : base(interpreter)
        {
            _repeater = repeater;
        }

        // TODO: Add support for synchronizers
        // TODO: Consider storing repeaters in a public stack to allow access to [first], [last], etc.
        public override bool Use()
        {
            return _repeater.Iterate(I, this);
        }
    }
}