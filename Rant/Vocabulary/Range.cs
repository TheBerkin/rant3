using System;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Defines a set of boundaries that can be used to enforce range requirements on numeric values.
	/// </summary>
	public class Range
	{
		private int? _min, _max;

		public int? Minimum
		{
			get { return _min; }
			set
			{
				if (value > _max)
					throw new ArgumentException("Minimum value must be less than or equal to maximum value.");
				_min = value;
			}
		}

		public int? Maximum
		{
			get { return _max; }
			set
			{
				if (value < _min)
					throw new ArgumentException("Maximum value must be greater than or equal to minimum value.");
				_max = value;
			}
		}

		public Range(int? min, int? max)
		{
			if (min > max)
				throw new ArgumentException("Maximum value must be greater than or equal to minimum value.");
			Minimum = min;
			Maximum = max;
		}

		/// <summary>
		/// Determines if the specified number is within the current range.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns></returns>
		public bool Test(int value)
		{
			if (_min == null && _max == null) return true;
			if (_min == null) return value <= _max;
			if (_max == null) return value >= _min;
			return value >= _min && value <= _max;
		}

		public static Range AtLeast(int min) => new Range(min, null);

		public static Range AtMost(int max) => new Range(null, max);

		public static Range Exactly(int number) => new Range(number, number);

		public static Range Between(int min, int max) => new Range(min, max);

		public static Range Anything => new Range(null, null);
	}
}