using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class REAConcatOperator : REAInfixOperator
	{
		public REAConcatOperator(Stringe origin)
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
				return new REAString(left, LeftSide.Range);
			}
			else if (leftVal is REAPatternString)
			{
				var left = (leftVal as REAPatternString);
				left.Value += ConvertValue(rightVal, sb);
                return left;
			}
			else
				throw new RantRuntimeException(sb.Pattern, base.Range, "Left side of concat operation must be a string or pattern.");
		}

		public string ConvertValue(object value, Sandbox sb)
		{
			if (value is REAPatternString)
				return (value as REAPatternString).Value;
			return value.ToString();
		}
	}
}