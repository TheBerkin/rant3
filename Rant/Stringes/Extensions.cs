using System;
using System.Text.RegularExpressions;

namespace Stringes
{
    public static class Extensions
    {
        /// <summary>
        /// Converts the specified value into a stringe.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns></returns>
        public static Stringe ToStringe(this object value)
        {
            return new Stringe(value.ToString());
        }

        public static bool IsMatch(this Regex regex, Stringe value, int startat = 0)
        {
            return regex.IsMatch(value.Value, startat);
        }

        public static Match Match(this Regex regex, Stringe value, int startat = 0)
        {
            return regex.Match(value.Value, startat);
        }

        public static string Slice(this string value, int a, int b)
        {
            if (b < a) throw new ArgumentException("'b' cannot be less tha 'a'.");
            if (b < 0 || a < 0) throw new ArgumentException("Indices cannot be negative.");
            if (a > value.Length || b > value.Length) throw new ArgumentException("Indices must be within string boundaries.");
            return a == b ? String.Empty : value.Substring(a, b - a);
        }
    }
}