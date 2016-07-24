using System.Collections.Generic;

using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard
{
	internal class RichArgumentSeperator : RichActionBase
	{
		public RichArgumentSeperator(Stringe _origin)
			: base(_origin)
		{
		}

		public override object GetValue(Sandbox sb)
		{
			return null;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield break;
		}
	}
}
