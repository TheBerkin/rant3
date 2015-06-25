using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class REAEqualityOperator : REAInfixOperator
	{
		public int Precedence = 0;

		public REAEqualityOperator(Stringe origin)
			: base(origin)
		{
			Type = ActionValueType.Boolean;
		}

		public override object GetValue(Sandbox sb)
		{
			var leftVal = sb.ScriptObjectStack.Pop();
			var rightVal = sb.ScriptObjectStack.Pop();

			return leftVal.GetHashCode() == rightVal.GetHashCode();
		}
	}
}
