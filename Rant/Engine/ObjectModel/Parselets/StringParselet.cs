using System;
using System.Collections.Generic;
using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
	internal class StringParselet : Parselet
	{
		public override IEnumerator<bool> Parse(Token<R> token, Rave vm)
		{
			vm.PushVal(new RantObject(Util.UnescapeConstantLiteral(token.Value)));
			yield break;
		}
	}
}