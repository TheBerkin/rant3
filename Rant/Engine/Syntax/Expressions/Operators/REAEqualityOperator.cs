using System;
using System.Collections.Generic;
using Rant.Stringes;
using Rant.Engine.ObjectModel;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class REAEqualityOperator : REAInfixOperator
	{
		public REAEqualityOperator(Stringe origin)
			: base(origin)
		{
			Type = ActionValueType.Boolean;
            Precedence = 15;
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
                    return true;
                return false;
            }

			return leftVal.GetHashCode() == rightVal.GetHashCode();
		}
	}
}
