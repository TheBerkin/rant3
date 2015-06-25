using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class REASubtractionOperator : REAInfixOperator
	{
		public new readonly int Precedence = 5;

		public REASubtractionOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x - y;
		}
	}
}
