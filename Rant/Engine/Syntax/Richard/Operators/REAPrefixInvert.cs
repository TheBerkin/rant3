using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
{
    internal class REAPrefixInvert : RantExpressionAction
    {
        public RantExpressionAction RightSide;

        public REAPrefixInvert(Stringe token)
            : base(token)
        {

        }

        public override object GetValue(Sandbox sb)
        {
            var rightVal = sb.ScriptObjectStack.Pop();
            if (!(rightVal is bool))
                throw new RantRuntimeException(sb.Pattern, Range,
                    "Right side of invert operator must be a boolean value.");
            bool rightBool = (bool)rightVal;
            return !rightBool;
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            var stackSize = sb.ScriptObjectStack.Count;
            yield return RightSide;
            if (stackSize >= sb.ScriptObjectStack.Count)
                throw new RantRuntimeException(sb.Pattern, Range, "Invalid right side of invert operator.");
            yield break;
        }
    }
}
