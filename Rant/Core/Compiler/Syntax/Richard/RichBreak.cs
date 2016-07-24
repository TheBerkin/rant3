using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
    internal class RichBreak : RichActionBase
    {
        public RichBreak(Stringe token)
            : base(token)
        {

        }

        public override object GetValue(Sandbox sb)
        {
            return null;
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield break;
        }
    }
}
