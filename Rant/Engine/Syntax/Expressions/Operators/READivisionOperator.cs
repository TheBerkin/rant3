using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class READivisionOperator : REAInfixOperator
	{
		public READivisionOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x / y;
            Precedence = 5;
        }
	}
}
