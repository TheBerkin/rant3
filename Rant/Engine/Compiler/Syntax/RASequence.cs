using System;
using System.Collections.Generic;

namespace Rant.Engine.Compiler.Syntax
{
	/// <summary>
	/// Performs a sequence of actions.
	/// </summary>
	internal class RASequence : RantAction
	{
		private readonly List<RantAction> _actions = new List<RantAction>();

		public RASequence(params RantAction[] actions)
		{
			if (actions == null) return;
			_actions.AddRange(actions);
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			foreach (var action in _actions) yield return action;
		}
	}
}