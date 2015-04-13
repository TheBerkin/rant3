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

		public List<RantAction> Actions => _actions;

		public RASequence(params RantAction[] actions)
			: base(actions.Any() ? Stringe.Range(actions[0].Range, actions[actions.Length - 1].Range) : null)
		{
			if (actions == null) return;
			_actions.AddRange(actions);
		}

		public RASequence(List<RantAction> actions)
			: base(actions.Any() ? Stringe.Range(actions[0].Range, actions[actions.Count - 1].Range) : null)
		{
			if (actions == null) return;
			_actions.AddRange(actions);
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			return _actions.GetEnumerator();
		}
	}
}