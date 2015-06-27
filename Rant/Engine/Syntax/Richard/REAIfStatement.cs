using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAIfStatement : RantExpressionAction
	{
		private RantExpressionAction _expression;
		private RantExpressionAction _body;
        private RantExpressionAction _elseBody;

		public REAIfStatement(Stringe token, RantExpressionAction expr, RantExpressionAction body, RantExpressionAction elseBody = null)
			: base(token)
		{
			_expression = expr;
			_body = body;
            _elseBody = elseBody;
		}

		public override object GetValue(Sandbox sb)
		{
			return null;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield return _expression;
			var result = sb.ScriptObjectStack.Pop();
			if (!(result is bool))
				throw new RantRuntimeException(sb.Pattern, Range, "Expected boolean value in if statement.");
			sb.Objects.EnterScope();
            if ((bool)result)
                yield return _body;
            else if (_elseBody != null)
                yield return _elseBody;
			sb.Objects.ExitScope();
			yield break;
		}
	}
}
