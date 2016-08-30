using System.Collections.Generic;
using System.Linq;

using Rant.Core.IO;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Performs a sequence of actions.
	/// </summary>
	[RST("patt")]
	internal class RstSequence : RST
	{
		public RstSequence(List<RST> actions, LineCol loc)
			: base(actions.Any() ? actions[0].Location : loc)
		{
			if (actions == null) return;
			Actions.AddRange(actions);
		}

		public RstSequence(LineCol location) : base(location)
		{
			// Used by serializer
		}

		public List<RST> Actions { get; } = new List<RST>();

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			return Actions.GetEnumerator();
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			output.Write(Actions.Count);
			foreach (var action in Actions) yield return action;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			int count = input.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var request = new DeserializeRequest();
				yield return request;
				Actions.Add(request.Result);
			}
		}
	}
}