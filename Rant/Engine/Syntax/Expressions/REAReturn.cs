using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REAReturn : RantExpressionAction
	{
		private RantExpressionAction _returnValue;

        public bool HasReturnValue = false;

		public REAReturn(Stringe origin, RantExpressionAction value)
			: base(origin)
		{
            if (value == null)
                HasReturnValue = false;
			_returnValue = value;
		}

		public override object GetValue(Sandbox sb)
		{
            return null;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
            if(_returnValue != null)
			    yield return _returnValue;
			yield break;
		}
	}
}
