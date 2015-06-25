using System;
using System.Collections.Generic;
using Rant.Stringes;
using Rant.Engine.ObjectModel;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REAVariable : RantExpressionAction
	{
		public string Name;

		public REAVariable(Stringe _origin)
			: base(_origin)
		{
			Name = _origin.Value;
		}
		
		public override object GetValue(Sandbox sb)
		{
            if (RichardFunctions.HasObject(Name))
                return RichardFunctions.GetObject(Name);
			if (sb.Objects[Name] == null) return new RantObject(false);
			var obj = sb.Objects[Name];
			return obj.Value;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield break;
		}
	}
}
