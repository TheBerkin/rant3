using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions
{
    // another thing just for interfacing with native functions
    // like an REAFunctionCall but executes a RantFunctionInfo
    internal class REAFunctionInfo : RantExpressionAction
    {
        RantFunctionInfo _function;

        public REAFunctionInfo(Stringe token, RantFunctionInfo function)
             : base(token)
        {
            _function = function;
        }

        public override object GetValue(Sandbox sb)
        {
            return null;
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            var iterator = _function.Invoke(sb, new object[] { });
            while (iterator.MoveNext())
                yield return iterator.Current;
            yield break;
        }
    }
}
