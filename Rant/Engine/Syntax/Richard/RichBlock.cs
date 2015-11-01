using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class RichBlock : RichActionBase
	{
		List<RichActionBase> _actions;

		public RichBlock(Stringe origin, List<RichActionBase> actions)
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
            foreach (RichActionBase action in _actions)
                yield return action;
			sb.Objects.ExitScope();
			yield break;
		}
	}
}
