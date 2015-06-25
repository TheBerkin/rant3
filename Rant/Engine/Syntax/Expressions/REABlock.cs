using Rant.Stringes;
using System;
using System.Collections.Generic;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REABlock : RantExpressionAction
	{
		List<RantExpressionAction> _actions;

		public REABlock(Stringe origin, List<RantExpressionAction> actions)
			: base(origin)
		{
			_actions = actions;
		}

		public override object GetValue(Sandbox sb)
		{
			return null;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			sb.Objects.EnterScope();
            foreach (RantExpressionAction action in _actions)
                yield return action;
			sb.Objects.ExitScope();
			yield break;
		}
	}
}
