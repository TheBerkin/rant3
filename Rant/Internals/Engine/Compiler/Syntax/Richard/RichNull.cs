using System.Collections.Generic;

using Rant.Internals.Engine.ObjectModel;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard
{
    internal class RichNull : RichActionBase
    {
        public RichNull(Stringe token)
            : base(token)
        {
        }

        public override object GetValue(Sandbox sb)
        {
            return new RantObject();
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield break;
        }
    }
}
