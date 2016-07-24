using System.Collections.Generic;
using System.Linq;

using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax
{
	/// <summary>
	/// Performs a sequence of actions.
	/// </summary>
	internal class RASequence : RantAction
	{
		private readonly List<RantAction> _actions = new List<RantAction>();

		public List<RantAction> Actions => _actions;

		public RASequence(List<RantAction> actions, Stringe defaultRange)
			: base(actions.Any() ? Stringe.Range(actions[0].Range, actions[actions.Count - 1].Range) : defaultRange)
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