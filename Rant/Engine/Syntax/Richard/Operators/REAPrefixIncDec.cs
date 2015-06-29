using System.Collections.Generic;

using Rant.Engine.ObjectModel;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
{
    internal class REAPrefixIncDec : RantExpressionAction
    {
        public RantExpressionAction RightSide;
        public bool Increment = true;

        public REAPrefixIncDec(Stringe token)
            : base(token)
        {

        }

        public override object GetValue(Sandbox sb)
        {
            if (!(RightSide is REAVariable))
                throw new RantRuntimeException(sb.Pattern, Range,
                    "Right side of prefix " + (Increment ? "increment" : "decrement") + " operator must be a variable.");
            string name = (RightSide as REAVariable).Name;
            double newValue = -1;
            if (sb.Objects[name] != null && sb.Objects[name].Type == RantObjectType.Number)
            {
                newValue = ((double)sb.Objects[name].Value);
                newValue = (Increment ? newValue + 1 : newValue - 1);
                sb.Objects[name] = new RantObject(newValue);
            }
            else if (sb.Objects[name] == null)
                throw new RantRuntimeException(sb.Pattern, Range, "Cannot increment undefined value.");
            else
                throw new RantRuntimeException(sb.Pattern, Range, "Cannot increment value of type " + sb.Objects[name].Type + ".");
            return newValue;
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield break;
        }
    }
}
