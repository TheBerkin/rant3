using System;

namespace Rant.Blueprints
{
    internal class FunctionBlueprint : Blueprint
    {
        private readonly Func<Interpreter, bool> _func; 
        public FunctionBlueprint(Interpreter interpreter, Func<Interpreter, bool> func) : base(interpreter)
        {
            _func = func;
        }

        public override bool Use()
        {
            return _func(I);
        }
    }
}