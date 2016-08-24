using System;

namespace Rant.Vocabulary.Querying
{
	/// <summary>
	/// Defines a set of boundaries that can be used to enforce range requirements on numeric values.
	/// </summary>
	public sealed class Range
	{
		private int? _min, _max;

		/// <summary>
		/// Creates a new Range object with the specified minimum and maximum bounds.
		/// </summary>
		/// <param name="min">The minimum bound.</param>
		/// <param name="max">The maximum bound.</param>
		public Range(int? min, int? max)
		{
			if (min > max)
				throw new ArgumentException("Maximum value must be greater than or equal to minimum value.");
			Minimum = min;
			Maximum = max;
		}

		/// <summary>
		/// Gets or sets the minimum bound of the range. Set this to null for no minimum.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the maximum bound of the range. Set this to null for no maximum.
		/// </summary>
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

		/// <summary>
		/// Creates a new Range object that accepts all values.
		/// </summary>
		public static Range Anything => new Range(null, null);

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

		/// <summary>
		/// Creates a new Range object with a minimum bound.
		/// </summary>
		/// <param name="min">The minimum bound.</param>
		/// <returns></returns>
		public static Range AtLeast(int min) => new Range(min, null);

		/// <summary>
		/// Creates a new Range object with a maximum bound.
		/// </summary>
		/// <param name="max">The maximum bound.</param>
		/// <returns></returns>
		public static Range AtMost(int max) => new Range(null, max);

		/// <summary>
		/// Creates a new Range object that accepts a single value.
		/// </summary>
		/// <param name="number">The value that the returned Range will accept.</param>
		/// <returns></returns>
		public static Range Exactly(int number) => new Range(number, number);

		/// <summary>
		/// Creates a new Range object with a minimum and maximum bound.
		/// </summary>
		/// <param name="min">The minimum bound.</param>
		/// <param name="max">The maximum bound.</param>
		/// <returns></returns>
		public static Range Between(int min, int max) => new Range(min, max);
	}
}