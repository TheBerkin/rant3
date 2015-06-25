using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class REALessThanOperator : REAInfixOperator
	{
		private bool _orEqual = false;

		public REALessThanOperator(Stringe origin, bool orEqual = false)
			: base(origin)
		{
			Type = ActionValueType.Boolean;
			_orEqual = orEqual;
            Precedence = 0;
        }

		public override object GetValue(Sandbox sb)
		{
			var leftVal = sb.ScriptObjectStack.Pop();
			var rightVal = sb.ScriptObjectStack.Pop();

			if (leftVal is double && rightVal is double)
				return (_orEqual ? (double)leftVal <= (double)rightVal : (double)leftVal < (double)rightVal);
			throw new RantRuntimeException(sb.Pattern, Range, "Invalid " + (leftVal is double ? "left hand" : "right hand") + " side of comparison operator.");
		}
	}
}
