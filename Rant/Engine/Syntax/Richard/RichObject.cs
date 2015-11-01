using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class RichObject : RichActionBase
	{
		public Dictionary<string, RichActionBase> Values;

        public RichObject(Stringe token)
            : base(token)
        {
            Type = ActionValueType.Object;
            Values = new Dictionary<string, RichActionBase>();
        }

		public RichObject(Stringe token, RichObjectKeyValue[] values)
			: base(token)
		{
			Type = ActionValueType.Object;
			Values = new Dictionary<string, RichActionBase>();
			foreach (RichObjectKeyValue val in values)
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
