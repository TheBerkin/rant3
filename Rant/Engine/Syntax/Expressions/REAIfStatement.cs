using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class REAIfStatement : RantExpressionAction
	{
		private RantExpressionAction _expression;
		private RantExpressionAction _body;

		public REAIfStatement(Stringe token, RantExpressionAction expr, RantExpressionAction body)
			: base(token)
		{
			_expression = expr;
			_body = body;
		}

		public override object GetValue(Sandbox sb)
		{
			return null;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield return _expression;
			var result = sb.ScriptObjectStack.Pop();
			if (result is bool && (bool)result == false)
				yield break;
			if (!(result is bool))
				throw new RantRuntimeException(sb.Pattern, Range, "Expected boolean value in if statement.");
			sb.Objects.EnterScope();
			yield return _body;
			sb.Objects.ExitScope();
			yield break;
		}
	}
}
