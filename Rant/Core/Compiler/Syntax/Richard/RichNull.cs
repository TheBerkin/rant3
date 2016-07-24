using System.Collections.Generic;

using Rant.Core.ObjectModel;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
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
