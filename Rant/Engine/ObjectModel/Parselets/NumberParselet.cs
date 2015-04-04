using System;
using Rant.Engine.Compiler;
using Rant.Stringes;
using System.Collections.Generic;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class NumberParselet : Parselet
	{
		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			vm.PushVal(new RantObject(Double.Parse(token.Value)));
			yield break;
		}
	}
}