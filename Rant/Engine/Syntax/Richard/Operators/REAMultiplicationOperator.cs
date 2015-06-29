using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
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
