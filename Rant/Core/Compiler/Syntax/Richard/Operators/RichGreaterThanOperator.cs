using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard.Operators
{
	internal class RichGreaterThanOperator : RichInfixOperator
	{
		private bool _orEqual = false;

		public RichGreaterThanOperator(Stringe origin, bool orEqual = false)
			: base(origin)
		{
			Type = ActionValueType.Boolean;
			_orEqual = orEqual;
            Precedence = 15;
        }

		public override object GetValue(Sandbox sb)
		{
			var leftVal = sb.ScriptObjectStack.Pop();
			var rightVal = sb.ScriptObjectStack.Pop();

			if (leftVal is double && rightVal is double)
				return (_orEqual ? (double)leftVal >= (double)rightVal : (double)leftVal > (double)rightVal);
			throw new RantRuntimeException(sb.Pattern, Range, "Invalid " + (leftVal is double ? "right hand" : "left hand") + " side of comparison operator.");
		}
	}
}
