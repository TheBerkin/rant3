using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
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
