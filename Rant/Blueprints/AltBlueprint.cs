using System.Collections;
using System.Collections.Generic;

using Rant.Compiler;

using Stringes.Tokens;

namespace Rant.Blueprints
{
    internal class AltBlueprint : Blueprint
    {
        private readonly long _old;
        private readonly Interpreter.State _target;
        private readonly IEnumerable<Token<TokenType>> _payload;
        public AltBlueprint(Interpreter interpreter, Interpreter.State target, IEnumerable<Token<TokenType>> payload) : base(interpreter)
        {
            _target = target;
            _payload = payload;
            _old = target.Output.Size;
        }

        public override bool Use()
        {
            if (_target.Output.Size > _old) return false;
            I.PushState(Interpreter.State.CreateDerivedDistinct(I.CurrentState.Reader.Source, _payload, I, I.CurrentState.Output));
            return false;
        }
    }
}