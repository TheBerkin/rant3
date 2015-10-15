using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class RichObjectKeyValue : RichActionBase
	{
		public readonly string Name;
		public readonly RichActionBase Value;

		public RichObjectKeyValue(Stringe token, RichActionBase value)
			: base(token)
		{
			Name = Util.UnescapeConstantLiteral(token.Value);
			Value = value;
		}

		public override object GetValue(Sandbox sb)
		{
			return Value;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield break;
		}
	}
}
