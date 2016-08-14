using Rant.Core.Stringes;

namespace Rant.Core.Compiler
{
	internal struct TokenLocation
	{
		public int Line;
		public int Column;
		public int Index;
		public int Length;

		public TokenLocation(int line, int column, int index, int length)
		{
			Line = line;
			Column = column;
			Index = index;
			Length = length;
		}

		public static TokenLocation FromStringe(Stringe stringe) => new TokenLocation(stringe.Line, stringe.Column, stringe.Offset, stringe.Length);
	}
}