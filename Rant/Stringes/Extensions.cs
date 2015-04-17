using System;
using System.Text.RegularExpressions;

namespace Rant.Stringes
{
	internal static class Extensions
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
    }
}