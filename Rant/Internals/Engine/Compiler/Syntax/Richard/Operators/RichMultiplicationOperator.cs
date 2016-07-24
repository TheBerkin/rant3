using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard.Operators
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
