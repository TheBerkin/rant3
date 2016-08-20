using Rant.Core.Stringes;

namespace Rant.Core.Compiler
{
	internal struct TokenLocation
	{
		public static readonly TokenLocation Unknown = new TokenLocation(-1, -1, -1);

		public int Line;
		public int Column;
		public int Index;

		public TokenLocation(int line, int column, int index)
		{
			Line = line;
			Column = column;
			Index = index;
		}

		public static TokenLocation FromStringe(Stringe stringe) => new TokenLocation(stringe.Line, stringe.Column, stringe.Offset);
	}
}