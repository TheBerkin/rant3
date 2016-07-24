using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
	internal class RichIfStatement : RichActionBase
	{
		private RichActionBase _expression;
		private RichActionBase _body;
        private RichActionBase _elseBody;

		public RichIfStatement(Stringe token, RichActionBase expr, RichActionBase body, RichActionBase elseBody = null)
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
