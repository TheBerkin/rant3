using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
    // dummy action for interfacing with native functions
    // just pushes value onto script object stack
    // this is necessary ok
    internal class RichDummy : RichActionBase
    {
        private object _object;

        public RichDummy(Stringe token, object obj)
            : base(token)
        {
            _object = obj;
        }

        public override object GetValue(Sandbox sb)
        {
            return _object;
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield break;
        }
    }
}
