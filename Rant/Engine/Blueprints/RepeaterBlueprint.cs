using Rant.Engine.Constructs;

namespace Rant.Engine.Blueprints
{
    internal sealed class RepeaterBlueprint : Blueprint
    {
        private readonly Repeater _repeater;

        public RepeaterBlueprint(VM interpreter, Repeater repeater)
            : base(interpreter)
        {
            _repeater = repeater;
        }

        public override bool Use()
        {
            bool status = _repeater.Iterate(I, this);
            if (!status) I.Objects.ExitScope();
            return status;
        }
    }
}