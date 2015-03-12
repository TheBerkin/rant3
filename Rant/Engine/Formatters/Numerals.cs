using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rant.Engine.Formatters
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

        private static readonly List<Tuple<long, string>> Powers;
        private static readonly List<Tuple<long, string>> Hundreds; 
        private static readonly List<Tuple<int, string>> TenUnits;
        private static readonly List<Tuple<int, string>> Tens;

        static Numerals()
        {
            Powers = new List<Tuple<long, string>>
            {
                Tuple.Create(100L, "hundred"),
                Tuple.Create(1000L, "thousand"),
                Tuple.Create(1000000L, "million"),
                Tuple.Create(1000000000L, "billion"),
                Tuple.Create(1000000000000L, "trillion"),
                Tuple.Create(1000000000000000L, "quadrillion"),
                Tuple.Create(1000000000000000000L, "quintillion")
            }.OrderByDescending(tuple => tuple.Item1).ToList();

            Hundreds = new List<Tuple<long, string>>
            {
                Tuple.Create(100L, "one hundred"),
                Tuple.Create(200L, "two hundred"),
                Tuple.Create(300L, "three hundred"),
                Tuple.Create(400L, "four hundred"),
                Tuple.Create(500L, "five hundred"),
                Tuple.Create(600L, "six hundred"),
                Tuple.Create(700L, "seven hundred"),
                Tuple.Create(800L, "eight hundred"),
                Tuple.Create(900L, "nine hundred"),
            }.OrderByDescending(tuple => tuple.Item1).ToList();

            TenUnits = new List<Tuple<int, string>>
            {
                Tuple.Create(90, "ninety"),
                Tuple.Create(80, "eighty"),
                Tuple.Create(70, "seventy"),
                Tuple.Create(60, "sixty"),
                Tuple.Create(50, "fifty"),
                Tuple.Create(40, "forty"),
                Tuple.Create(30, "thirty"),
                Tuple.Create(20, "twenty")
            }.OrderByDescending(tuple => tuple.Item1).ToList();

            Tens = new List<Tuple<int, string>>
            {
                Tuple.Create(19, "nineteen"),
                Tuple.Create(18, "eighteen"),
                Tuple.Create(17, "seventeen"),
                Tuple.Create(16, "sixteen"),
                Tuple.Create(15, "fifteen"),
                Tuple.Create(14, "fourteen"),
                Tuple.Create(13, "thirteen"),
                Tuple.Create(12, "twelve"),
                Tuple.Create(11, "eleven"),
                Tuple.Create(10, "ten"),
                Tuple.Create(9, "nine"),
                Tuple.Create(8, "eight"),
                Tuple.Create(7, "seven"),
                Tuple.Create(6, "six"),
                Tuple.Create(5, "five"),
                Tuple.Create(4, "four"),
                Tuple.Create(3, "three"),
                Tuple.Create(2, "two"),
                Tuple.Create(1, "one")
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
            var input = new Queue<Tuple<long, string>>();

            long unit = 0;

            foreach (var power in Powers)
            {
                if (number < power.Item1) continue;
                unit = number / power.Item1; // Count of current unit (1-999)
                number -= unit * power.Item1; // Strip the units from the number
                input.Enqueue(Tuple.Create(unit, power.Item2));
            }
            
            input.Enqueue(Tuple.Create(number, ""));

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