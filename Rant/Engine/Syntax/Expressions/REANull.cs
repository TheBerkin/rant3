using System;
using Rant.Stringes;
using Rant.Engine.ObjectModel;
using System.Collections.Generic;

namespace Rant.Engine.Syntax.Expressions
{
    internal class REANull : RantExpressionAction
    {
        public REANull(Stringe token)
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
