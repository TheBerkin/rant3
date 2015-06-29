using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
{
	internal class REASubtractionOperator : REAInfixOperator
	{
		public REASubtractionOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x - y;
            Precedence = 10;
        }
	}
}
