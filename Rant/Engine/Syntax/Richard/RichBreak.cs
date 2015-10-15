using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
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
