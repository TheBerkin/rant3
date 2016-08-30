namespace Rant.Core.Compiler
{
	internal struct LineCol
	{
		public static readonly LineCol Unknown = new LineCol(-1, -1, -1);
		public int Column;
		public int Index;
		public int Line;

		public LineCol(int line, int column, int index)
		{
			Line = line;
			Column = column;
			Index = index;
		}
	}
}