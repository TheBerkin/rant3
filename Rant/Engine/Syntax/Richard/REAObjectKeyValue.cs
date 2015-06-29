using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAObjectKeyValue : RantExpressionAction
	{
		public readonly string Name;
		public readonly RantExpressionAction Value;

		public REAObjectKeyValue(Stringe token, RantExpressionAction value)
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
