using System;
using System.Collections.Generic;

using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard.Operators
{
	internal class RichInfixOperator : RichActionBase
	{
		public RichActionBase LeftSide;
		public RichActionBase RightSide;

		public Func<double, double, double> Operation;

	    internal int Precedence = 0;

		private Stringe Origin;

		public RichInfixOperator(Stringe _origin)
			: base(_origin)
		{
			Origin = _origin;
			Type = ActionValueType.Number;
		}

		public override object GetValue(Sandbox sb)
		{
			if (LeftSide == null || RightSide == null)
				throw new RantRuntimeException(sb.Pattern, Origin, "Both sides of infix operation must be defined.");
			var leftValue = sb.ScriptObjectStack.Pop();
			var rightValue = sb.ScriptObjectStack.Pop();
            if (!(leftValue is double))
				throw new RantRuntimeException(sb.Pattern, Origin, "Left side of infix operation must be a number.");
			if (!(rightValue is double))
				throw new RantRuntimeException(sb.Pattern, Origin, "Right side of infix operation must be a number.");
            if (this is RichDivisionOperator && (double)rightValue == 0)
                throw new RantRuntimeException(sb.Pattern, Origin, "Cannot divide by zero.");
            return Operation((double)leftValue, (double)rightValue);
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
            if (LeftSide == null || RightSide == null)
                throw new RantRuntimeException(sb.Pattern, Origin, "Missing part of infix operation.");
			yield return RightSide;
			yield return LeftSide;
			yield break;
		}
	}
}
