using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAReturn : RantExpressionAction
	{
		private RantExpressionAction _returnValue;
        public bool HasReturnValue => _returnValue != null;

		public REAReturn(Stringe origin, RantExpressionAction value)
			: base(origin)
		{
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
