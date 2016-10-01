using System;

using Rant.Core.IO;

namespace Rant.Vocabulary.Querying
{
	/// <summary>
	/// Defines a set of boundaries that can be used to enforce range requirements on numeric values.
	/// </summary>
	internal sealed class RangeFilter : Filter
	{
		private int? _min, _max;

		/// <summary>
		/// Creates a new Range object with the specified minimum and maximum bounds.
		/// </summary>
		/// <param name="min">The minimum bound.</param>
		/// <param name="max">The maximum bound.</param>
		public RangeFilter(int? min, int? max)
		{
			if (min > max)
				throw new ArgumentException("Maximum value must be greater than or equal to minimum value.");
			Minimum = min;
			Maximum = max;
		}

		public RangeFilter()
		{
			// Used by serializer
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
		public static RangeFilter Anything => new RangeFilter(null, null);

		/// <summary>
		/// Determines if the specified number is within the current range.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns></returns>
		public bool TestAgainst(int value)
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
		public static RangeFilter AtLeast(int min) => new RangeFilter(min, null);

		/// <summary>
		/// Creates a new Range object with a maximum bound.
		/// </summary>
		/// <param name="max">The maximum bound.</param>
		/// <returns></returns>
		public static RangeFilter AtMost(int max) => new RangeFilter(null, max);

		/// <summary>
		/// Creates a new Range object that accepts a single value.
		/// </summary>
		/// <param name="number">The value that the returned Range will accept.</param>
		/// <returns></returns>
		public static RangeFilter Exactly(int number) => new RangeFilter(number, number);

		/// <summary>
		/// Creates a new Range object with a minimum and maximum bound.
		/// </summary>
		/// <param name="min">The minimum bound.</param>
		/// <param name="max">The maximum bound.</param>
		/// <returns></returns>
		public static RangeFilter Between(int min, int max) => new RangeFilter(min, max);

		public override bool Test(RantDictionary dictionary, RantDictionaryTable table, RantDictionaryEntry entry, int termIndex, Query query) => TestAgainst(entry[termIndex].SyllableCount);
		public override int Priority => 1;

		public override void Deserialize(EasyReader input)
		{
			if (input.ReadBoolean()) _min = input.ReadInt32();
			if (input.ReadBoolean()) _max = input.ReadInt32();
		}

		public override void Serialize(EasyWriter output)
		{
			output.Write(FILTER_SYLRANGE);
			if (_min == null)
			{
				output.Write(false);
			}
			else
			{
				output.Write(true);
				output.Write(_min.Value);
			}

			if (_max == null)
			{
				output.Write(false);
			}
			else
			{
				output.Write(true);
				output.Write(_max.Value);
			}
		}
	}
}