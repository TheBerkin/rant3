using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
    // another thing just for interfacing with native functions
    // like an REAFunctionCall but executes a RantFunctionInfo
    internal class RichFunctionInfo : RichActionBase
    {
        RantFunctionInfo _function;

        public RantFunctionInfo Function => _function;

        public RichFunctionInfo(Stringe token, RantFunctionInfo function)
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
