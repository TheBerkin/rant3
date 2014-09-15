namespace Rant.Blueprints
{
    internal class RepeaterStackBlueprint : Blueprint
    {
        private readonly Repeater _rep;
        private readonly RepeaterStackAction _action;
        public RepeaterStackBlueprint(Interpreter interpreter, Repeater repeater, RepeaterStackAction action) : base(interpreter)
        {
            _rep = repeater;
            _action = action;
        }

        public override bool Use()
        {
            switch (_action)
            {
                case RepeaterStackAction.Push:
                    I.PushRepeater(_rep);
                    break;
                case RepeaterStackAction.Pop:
                    I.PopRepeater();
                    break;
            }
            return false;
        }
    }

    internal enum RepeaterStackAction
    {
        Push,
        Pop
    }
}