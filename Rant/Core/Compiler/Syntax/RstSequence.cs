using System.Collections.Generic;
using System.Linq;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Performs a sequence of actions.
	/// </summary>
	internal class RstSequence : RST
	{
		private readonly List<RST> _actions = new List<RST>();

		public List<RST> Actions => _actions;

		public RstSequence(List<RST> actions, Stringe defaultRange)
			: base(actions.Any() ? actions[0].Location : TokenLocation.FromStringe(defaultRange))
		{
			if (actions == null) return;
			_actions.AddRange(actions);
		}

		public RstSequence(List<RST> actions, TokenLocation loc)
			: base(actions.Any() ? actions[0].Location : loc)
		{
			if (actions == null) return;
			_actions.AddRange(actions);
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			return _actions.GetEnumerator();
		}
	}
}