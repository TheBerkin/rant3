using System.Collections.Generic;

using Rant.Core.Framework;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
    // another thing just for interfacing with native functions
    // like an REAFunctionCall but executes a RantFunctionInfo
    internal class RichFunctionInfo : RichActionBase
    {
        RantFunctionSignature _function;

        public RantFunctionSignature Function => _function;

        public RichFunctionInfo(Stringe token, RantFunctionSignature function)
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
