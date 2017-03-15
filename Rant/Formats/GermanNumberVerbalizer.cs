using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant.Formats
{
	public sealed class GermanNumberVerbalizer : NumberVerbalizer
	{
		private readonly string[] thousandPowers = { "tausend", "eine Million ", "eine Milliarde ", "eine Billion ", "eine Billiarde ", "eine Trillion ", "eine Trilliarde ", "eine Quadrillion ", "eine Quadrilliarde " };
		private readonly string[] thousandPowersPl = { "tausend", " Millionen ", " Milliarden ", " Billionen ", " Billiarden ", " Trillionen ", " Trilliarden ", " Quadrillionen ", " Quadrilliarden " };

		private static readonly string[] cache;

		static GermanNumberVerbalizer()
		{
			string[] tens = { "", "zehn", "zwanzig", "dreißig", "vierzig", "fünfzig", "sechzig", "siebzig", "achtzig", "neunzig" };
			string[] und = { "", "einund", "zweiund", "dreiund", "vierund", "fünfund", "sechsund", "siebenund", "achtund", "neunund" };
			string[] pre = { "null", "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun",
							 "zehn", "elf", "zwölf", "dreizehn", "vierzehn", "fünfzehn", "sechzehn", "siebzehn", "achtzehn", "neunzehn" };
			cache = new string[1000];
			for (int i = 0; i < 1000; i++)
			{
				if (i < 20)
				{
					cache[i] = pre[i];
				}
				else if (i < 100)
				{
					cache[i] = (i % 10 > 0 ? und[i % 10] : "") + tens[i / 10];
				}
				else
				{
					int h = i % 100;
					cache[i] = (i / 100 > 1 ? pre[i / 100] : "") + "hundert"
						+ (h > 0
							? (h < 20
									? pre[h]
									: (h % 10 > 0 ? und[i % 10] : "") + tens[h / 10])
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
				sb.Append("minus ");
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
				if (p > 0)
				{
					if (gv == 1	 && p > 1)
					{
						// One million +
						sb.Append(thousandPowers[p - 1]);
					}
					else
					{
						// Print powers of thousand
						if (gv > 1) sb.Append(cache[gv]);
						if (gv > 0) sb.Append(thousandPowersPl[p - 1]);						
					}
					
				}
				else if ((gn > 1 && gv != 0) || gn == 1)
				{
					// Print last three digits
					sb.Append(cache[gv]);
				}
			}
			return sb.ToString().Trim();
		}
	}
}
