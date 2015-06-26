using Rant.Engine.ObjectModel;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
{
    internal class REAInequalityOperator : REAInfixOperator
    {
        public REAInequalityOperator(Stringe origin)
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

            return leftVal.GetHashCode() != rightVal.GetHashCode();
        }
    }
}
