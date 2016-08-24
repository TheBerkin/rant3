namespace Rant.Core.Utilities
{
	internal sealed class Limit
	{
		private int _value;

		public Limit(int max)
		{
			Maximum = max;
			_value = 0;
		}

		public int Maximum { get; }

		public bool Accumulate(int value)
		{
			return Maximum > 0 && (_value += value) > Maximum;
		}
	}
}