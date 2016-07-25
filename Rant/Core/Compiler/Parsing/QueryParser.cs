using System;
using System.Collections.Generic;

using Rant.Core.Compiler.Syntax;

namespace Rant.Core.Compiler.Parsing
{
	internal class QueryParser : Parser
	{
		public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RantAction> actionCallback)
		{
			throw new NotImplementedException();
		}
	}
}