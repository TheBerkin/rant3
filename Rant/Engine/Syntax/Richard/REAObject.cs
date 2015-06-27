using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAObject : RantExpressionAction
	{
		public Dictionary<string, RantExpressionAction> Values;

        public REAObject(Stringe token)
            : base(token)
        {
            Type = ActionValueType.Object;
            Values = new Dictionary<string, RantExpressionAction>();
        }

		public REAObject(Stringe token, REAObjectKeyValue[] values)
			: base(token)
		{
			Type = ActionValueType.Object;
			Values = new Dictionary<string, RantExpressionAction>();
			foreach (REAObjectKeyValue val in values)
				Values[val.Name] = val.Value;
		}

		public override object GetValue(Sandbox sb)
		{
			return this;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield break;
		}

		public override string ToString()
		{
			return "object";
		}
	}
}
