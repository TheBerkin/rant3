using System.Collections.Generic;
using Rant.Compiler;
using Rant.Stringes.Tokens;

namespace Rant.Blueprints
{
    internal class AnyBlueprint : Blueprint
    {
        private readonly long _old;
        private readonly VM.State _target;
        private readonly IEnumerable<Token<R>> _payload;

        public AnyBlueprint(VM interpreter, VM.State target, IEnumerable<Token<R>> payload)
            : base(interpreter)
        {
            _target = target;
            _payload = payload;
            _old = _target.Output.Size;
        }

        public override bool Use()
        {
            if (_target.Output.Size == _old) return false;
            I.PushState(VM.State.CreateSub(I.CurrentState.Reader.Source, _payload, I, I.CurrentState.Output));
            return false;
        }
    }
}