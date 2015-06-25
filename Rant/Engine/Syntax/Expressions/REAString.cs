using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REAString : RantExpressionAction
	{
		public string Value;

		public REAString(string _value, Stringe _origin)
			: base(_origin)
		{
			Value = _value;
			Type = ActionValueType.String;
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
			return Value;
		}
	}
}
