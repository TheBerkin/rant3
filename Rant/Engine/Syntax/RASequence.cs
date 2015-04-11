using Rant.Stringes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Engine.Syntax
{
	/// <summary>
	/// Performs a sequence of actions.
	/// </summary>
	internal class RASequence : RantAction
	{
		private readonly List<RantAction> _actions = new List<RantAction>();

		public RASequence(params RantAction[] actions)
			: base(actions.Any() ? Stringe.Range(actions[0].Stringe, actions[actions.Length - 1].Stringe) : null)
		{
			if (actions == null) return;
			_actions.AddRange(actions);
		}

		public RASequence(List<RantAction> actions)
			: base(actions.Any() ? Stringe.Range(actions[0].Stringe, actions[actions.Count - 1].Stringe) : null)
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