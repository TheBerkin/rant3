using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rant.Core.Formatting
{
	internal static class Numerals
	{
		private static readonly string[][] RomanNumerals =
		{
			new [] {"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"},
			new [] {"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"},
			new [] {"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"},
			new [] {"", "M", "MM", "MMM"}
		};

		public const int MaxRomanValue = 3999;

		private static readonly List<_<long, string>> Powers;
		private static readonly List<_<long, string>> Hundreds;
		private static readonly List<_<int, string>> TenUnits;
		private static readonly List<_<int, string>> Tens;

		static Numerals()
		{
			Powers = new List<_<long, string>>
			{
				_.Create(100L, "hundred"),
				_.Create(1000L, "thousand"),
				_.Create(1000000L, "million"),
				_.Create(1000000000L, "billion"),
				_.Create(1000000000000L, "trillion"),
				_.Create(1000000000000000L, "quadrillion"),
				_.Create(1000000000000000000L, "quintillion")
			}.OrderByDescending(tuple => tuple.Item1).ToList();

			Hundreds = new List<_<long, string>>
			{
				_.Create(100L, "one hundred"),
				_.Create(200L, "two hundred"),
				_.Create(300L, "three hundred"),
				_.Create(400L, "four hundred"),
				_.Create(500L, "five hundred"),
				_.Create(600L, "six hundred"),
				_.Create(700L, "seven hundred"),
				_.Create(800L, "eight hundred"),
				_.Create(900L, "nine hundred"),
			}.OrderByDescending(tuple => tuple.Item1).ToList();

			TenUnits = new List<_<int, string>>
			{
				_.Create(90, "ninety"),
				_.Create(80, "eighty"),
				_.Create(70, "seventy"),
				_.Create(60, "sixty"),
				_.Create(50, "fifty"),
				_.Create(40, "forty"),
				_.Create(30, "thirty"),
				_.Create(20, "twenty")
			}.OrderByDescending(tuple => tuple.Item1).ToList();

			Tens = new List<_<int, string>>
			{
				_.Create(19, "nineteen"),
				_.Create(18, "eighteen"),
				_.Create(17, "seventeen"),
				_.Create(16, "sixteen"),
				_.Create(15, "fifteen"),
				_.Create(14, "fourteen"),
				_.Create(13, "thirteen"),
				_.Create(12, "twelve"),
				_.Create(11, "eleven"),
				_.Create(10, "ten"),
				_.Create(9, "nine"),
				_.Create(8, "eight"),
				_.Create(7, "seven"),
				_.Create(6, "six"),
				_.Create(5, "five"),
				_.Create(4, "four"),
				_.Create(3, "three"),
				_.Create(2, "two"),
				_.Create(1, "one")
			}.OrderByDescending(tuple => tuple.Item1).ToList();
		}

		public static string ToRoman(double number, bool lowerCase = false)
		{
			if (number <= 0 || number > MaxRomanValue || number % 1 > 0) return "?";
			var intArr = number.ToString(CultureInfo.InvariantCulture).Reverse().Select(c => Int32.Parse(c.ToString(CultureInfo.InvariantCulture))).ToArray();
			var sb = new StringBuilder();
			for (int i = intArr.Length; i-- > 0;)
			{
				sb.Append(RomanNumerals[i][intArr[i]]);
			}
			return lowerCase ? sb.ToString().ToLower() : sb.ToString();
		}

		public static string ToVerbal(long number)
		{
			if (number == 0) return "zero";

			var numBuilder = new StringBuilder();

			if (number < 0)
			{
				numBuilder.Append("negative ");
				number *= -1;
			}

			// The input stack contains number units for the loop to translate
			var input = new Queue<_<long, string>>();

			long unit = 0;

			foreach (var power in Powers)
			{
				if (number < power.Item1) continue;
				unit = number / power.Item1; // Count of current unit (1-999)
				number -= unit * power.Item1; // Strip the units from the number
				input.Enqueue(_.Create(unit, power.Item2));
			}

			input.Enqueue(_.Create(number, ""));

			var buffer = new StringBuilder();

			while (input.Any())
			{
				var pair = input.Dequeue();
				long value = pair.Item1;
				if (value == 0) continue;

				if (numBuilder.Length > 0)
				{
					if (input.Any())
					{
						numBuilder.Append(", ");
					}
					else if (value > 0)
					{
						numBuilder.Append(" and ");
					}
				}

				buffer.Length = 0;

				foreach (var hundred in Hundreds)
				{
					if (value < hundred.Item1) continue;
					value -= hundred.Item1;
					buffer.AppendFormat("{0}{1}{2}", buffer.Length > 0 ? " " : String.Empty, hundred.Item2, value > 0 ? " and" : String.Empty);
				}

				foreach (var tenunit in TenUnits)
				{
					if (value < tenunit.Item1) continue;
					value -= tenunit.Item1;
					buffer.AppendFormat("{0}{1}", buffer.Length > 0 ? " " : String.Empty, tenunit.Item2);
					break;
				}

				foreach (var ten in Tens)
				{
					if (value != ten.Item1) continue;
					buffer.AppendFormat("{0}{1}", buffer.Length > 0 ? " " : String.Empty, ten.Item2);
					break;
				}

				numBuilder.Append(buffer);
				if (pair.Item2.Length > 0)
				{
					numBuilder.Append(" ").Append(pair.Item2);
				}
			}
			return numBuilder.ToString();
		}
	}
}