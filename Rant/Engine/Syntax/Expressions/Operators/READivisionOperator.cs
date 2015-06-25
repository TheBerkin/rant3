using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class READivisionOperator : REAInfixOperator
	{
		public new readonly int Precedence = 10;

		public READivisionOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x / y;
		}
	}
}
