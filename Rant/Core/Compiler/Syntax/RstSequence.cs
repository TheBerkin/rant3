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
		public RstSequence(List<RST> actions, Stringe defaultRange)
			: base(actions.Any() ? actions[0].Location : TokenLocation.FromStringe(defaultRange))
		{
			if (actions == null) return;
			Actions.AddRange(actions);
		}

		public RstSequence(List<RST> actions, TokenLocation loc)
			: base(actions.Any() ? actions[0].Location : loc)
		{
			if (actions == null) return;
			Actions.AddRange(actions);
		}

		public List<RST> Actions { get; } = new List<RST>();

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			return Actions.GetEnumerator();
		}
	}
}