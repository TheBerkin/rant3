using System;

namespace Rant.Blueprints
{
    internal class DelegateBlueprint : Blueprint
    {
        private readonly Func<Interpreter, bool> _func; 
        public DelegateBlueprint(Interpreter interpreter, Func<Interpreter, bool> func) : base(interpreter)
        {
            _func = func;
        }

        public override bool Use()
        {
            return _func(I);
        }
    }
}