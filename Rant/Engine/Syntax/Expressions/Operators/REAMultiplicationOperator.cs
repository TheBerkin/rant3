using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions.Operators
{
	internal class REAMultiplicationOperator : REAInfixOperator
	{
		public REAMultiplicationOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x * y;
            Precedence = 5;
        }
	}
}
