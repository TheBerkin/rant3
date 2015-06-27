using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REANumber : RantExpressionAction
	{
		public double Value;

		public REANumber(double _value, Stringe _origin)
			: base(_origin)
		{
			Value = _value;
			Type = ActionValueType.Number;
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
            return Value.ToString();
        }
    }
}
