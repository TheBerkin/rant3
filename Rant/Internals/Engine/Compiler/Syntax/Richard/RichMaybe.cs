using System.Collections.Generic;

using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard
{
	internal class RichMaybe : RichActionBase
	{
		public RichMaybe(Stringe token)
			: base(token)
		{
			Type = ActionValueType.Boolean;
		}

		public override object GetValue(Sandbox sb)
		{
			return sb.RNG.NextBoolean();
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield break;
		}
	}
}
