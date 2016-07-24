using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
	internal class RichReturn : RichActionBase
	{
		private RichActionBase _returnValue;
        public bool HasReturnValue => _returnValue != null;

		public RichReturn(Stringe origin, RichActionBase value)
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
