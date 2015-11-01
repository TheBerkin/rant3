using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard.Operators
{
	internal class RichMultiplicationOperator : RichInfixOperator
	{
		public RichMultiplicationOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x * y;
            Precedence = 5;
        }
	}
}
