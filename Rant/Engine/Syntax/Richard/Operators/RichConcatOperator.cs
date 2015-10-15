using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
{
	internal class RichConcatOperator : RichInfixOperator
	{
		public RichConcatOperator(Stringe origin)
			: base(origin)
		{
			Type = ActionValueType.Null;
            Precedence = 1;
        }

		public override object GetValue(Sandbox sb)
		{
			var leftVal = sb.ScriptObjectStack.Pop();
			var rightVal = sb.ScriptObjectStack.Pop();
			if (leftVal is string)
			{
				var left = (leftVal as string);
				left += ConvertValue(rightVal, sb);
                return left;
			}
			else if (leftVal is RichPatternString)
			{
				var left = (leftVal as RichPatternString);
				left.Value += ConvertValue(rightVal, sb);
                return left;
			}
			else
				throw new RantRuntimeException(sb.Pattern, base.Range, "Left side of concat operation must be a string or pattern.");
		}

		public string ConvertValue(object value, Sandbox sb)
		{
			if (value is RichPatternString)
				return (value as RichPatternString).Value;
			return value.ToString();
		}
	}
}