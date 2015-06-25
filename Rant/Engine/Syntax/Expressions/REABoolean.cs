using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REABoolean : RantExpressionAction
	{
		public readonly bool Value;

		public REABoolean(Stringe token, bool value)
			: base(token)
		{
			Value = value;
			Type = ActionValueType.Boolean;
		}

		public override object GetValue(Sandbox sb)
		{
			return Value;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield break;
		}

		public override string ToString()
		{
			return Value ? "yes" : "no";
		}
	}
}
