using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard.Operators
{
	internal class RichDivisionOperator : RichInfixOperator
	{
		public RichDivisionOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x / y;
            Precedence = 5;
        }
	}
}
