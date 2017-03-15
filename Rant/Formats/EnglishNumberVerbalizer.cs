using System;
using System.Globalization;
using System.Text;

namespace Rant.Formats
{
	public sealed class EnglishNumberVerbalizer : NumberVerbalizer
	{
		private readonly string[] thousandPowers = { "thousand", "million", "billion", "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion" };

		private static readonly string[] cache;

		static EnglishNumberVerbalizer()
		{
			string[] tens = { "", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
			string[] pre = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
							 "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
			cache = new string[1000];
			for(int i = 0; i < 1000; i++)
			{
				if (i < 20)
				{
					cache[i] = pre[i];
				}
				else if (i < 100)
				{
					cache[i] = tens[i / 10] + (i % 10 > 0 ? $"-{pre[i % 10]}" : "");
				}
				else
				{
					int h = i % 100;
					cache[i] = pre[i / 100] + " hundred"
						+ (h > 0
							? " and " 
								+ (h < 20 
									? pre[h] 
									: tens[h / 10] + (h % 10 > 0 ? "-" + pre[i % 10] : ""))
							: "");
				}
			}
		}

		public override string Verbalize(long number)
		{
			var sb = new StringBuilder();

			if (number < 0)
			{
				number = -number;
				sb.Append("negative ");
			}

			var nstr = Convert.ToString(number, CultureInfo.InvariantCulture);
			int dn = nstr.Length;
			int firstGroupLength = dn % 3;
			if (firstGroupLength == 0) firstGroupLength = 3;
			int gn = (int)Math.Ceiling(dn / 3f);
			string group;

			for (int i = 0; i < gn; i++)
			{
				group = i == 0
						? nstr.Substring(0, firstGroupLength)
						: nstr.Substring(firstGroupLength + (i - 1) * 3, 3);

				// Index of power name
				int p = (gn - 1) - i;
				int gv = Int32.Parse(group);
				if (p > 0 && gv > 0)
				{
					// Print powers of thousand
					sb.Append($"{cache[gv]} {thousandPowers[p - 1]}");

					// If the last three digits are 000, there's no space needed.
					if (!(p - 1 == 0 && number % 1000 == 0)) sb.Append(" ");
				}
				else if ((gn > 1 && gv != 0) || gn == 1)
				{
					if (gn > 1 && gv < 100) sb.Append("and ");
					// Print last three digits
					sb.Append(cache[gv]);
				}
			}
			return sb.ToString();
		}
	}
}
