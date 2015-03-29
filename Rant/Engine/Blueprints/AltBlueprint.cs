using System.Collections.Generic;

using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.Blueprints
{
    internal class AltBlueprint : Blueprint
    {
        private readonly long _old;
        private readonly RantState _target;
        private readonly IEnumerable<Token<R>> _payload;
        public AltBlueprint(VM interpreter, RantState target, IEnumerable<Token<R>> payload) : base(interpreter)
        {
            _target = target;
            _payload = payload;
            _old = target.Output.Size;
        }

        public override bool Use()
        {
            if (_target.Output.Size > _old) return false;
            I.PushState(RantState.CreateSub(I.CurrentState.Reader.Source, _payload, I, I.CurrentState.Output));
            return false;
        }
    }
}