using Rant.Core.ObjectModel;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard.Operators
{
    internal class RichInequalityOperator : RichInfixOperator
    {
        public RichInequalityOperator(Stringe origin)
            : base(origin)
        {
            Type = ActionValueType.Boolean;
            Precedence = 0;
        }

        public override object GetValue(Sandbox sb)
        {
            var leftVal = sb.ScriptObjectStack.Pop();
            var rightVal = sb.ScriptObjectStack.Pop();
            if (leftVal is RantObject)
                leftVal = (leftVal as RantObject).Value;
            if (rightVal is RantObject)
                rightVal = (rightVal as RantObject).Value;
            if (leftVal == null || rightVal == null)
            {
                if (leftVal == null && rightVal == null)
                    return false;
                return true;
            }

            return leftVal.GetHashCode() != rightVal.GetHashCode();
        }
    }
}
