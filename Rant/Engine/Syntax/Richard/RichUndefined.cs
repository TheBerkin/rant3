using System.Collections.Generic;

using Rant.Engine.ObjectModel;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
    internal class RichUndefined : RichActionBase
    {
        public RichUndefined(Stringe token)
            : base(token)
        {

        }

        public override object GetValue(Sandbox sb)
        {
            return new RantObject(RantObjectType.Undefined);
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield break;
        }
    }
}
