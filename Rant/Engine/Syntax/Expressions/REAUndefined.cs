using System;
using System.Collections.Generic;
using Rant.Engine.ObjectModel;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions
{
    internal class REAUndefined : RantExpressionAction
    {
        public REAUndefined(Stringe token)
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
