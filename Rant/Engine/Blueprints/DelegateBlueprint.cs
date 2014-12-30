using System;

namespace Rant.Engine.Blueprints
{
    internal class DelegateBlueprint : Blueprint
    {
        private readonly Func<VM, bool> _func; 
        public DelegateBlueprint(VM interpreter, Func<VM, bool> func) : base(interpreter)
        {
            _func = func;
        }

        public override bool Use()
        {
            return _func(I);
        }
    }
}