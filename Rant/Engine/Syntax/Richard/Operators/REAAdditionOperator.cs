using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
{
	internal class REAAdditionOperator : REAInfixOperator
	{
		public REAAdditionOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x + y;
            Precedence = 10;
        }
	}
}
