using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard.Operators
{
	internal class RichSubtractionOperator : RichInfixOperator
	{
		public RichSubtractionOperator(Stringe _origin)
			: base(_origin)
		{
			Operation = (x, y) => x - y;
            Precedence = 10;
        }
	}
}
