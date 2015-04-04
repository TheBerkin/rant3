using System;
using System.Collections.Generic;
using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class EscapeParselet : Parselet
	{
		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			vm.PushVal(new RantObject(Util.Unescape(token.Value, vm.Rant, vm.Rant.RNG)));
			yield break;
		}
	}
}