using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class REAMultiplicationOperator : REAInfixOperator
	{
		public new readonly int Precedence = 10;

		public REAMultiplicationOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x * y;
		}
	}
}
