using System.Collections.Generic;

using Rant.Engine.ObjectModel;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
{
    internal class RichPostfixIncDec : RichActionBase
    {
        public RichActionBase LeftSide;
        public bool Increment = true;

        public RichPostfixIncDec(Stringe token)
            : base(token)
        {

        }

        public override object GetValue(Sandbox sb)
        {
            if (!(LeftSide is RichVariable))
                throw new RantRuntimeException(sb.Pattern, Range,
                    "Right side of prefix " + (Increment ? "increment" : "decrement") + " operator must be a variable.");
            string name = (LeftSide as RichVariable).Name;
            double prevValue = -1;
            if (sb.Objects[name] != null && sb.Objects[name].Type == RantObjectType.Number)
            {
                prevValue = ((double)sb.Objects[name].Value);
                sb.Objects[name] = new RantObject(Increment ? prevValue + 1 : prevValue - 1);
            }
            else if (sb.Objects[name] == null)
                throw new RantRuntimeException(sb.Pattern, Range, "Cannot increment undefined value.");
            else
                throw new RantRuntimeException(sb.Pattern, Range, "Cannot increment value of type " + sb.Objects[name].Type + ".");
            return prevValue;
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield break;
        }
    }
}
