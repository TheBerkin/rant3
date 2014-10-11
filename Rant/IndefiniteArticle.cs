using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rant
{
    /// <summary>
    /// Defines indefinite article (\a) formatting to use when executing Rant patterns.
    /// </summary>
    public sealed class IndefiniteArticle
    {
        /// <summary>
        /// Indefinite articles for English.
        /// </summary>
        public static readonly IndefiniteArticle English = new IndefiniteArticle("a", "an", "uni", "use", "U.", "one");

        private readonly string _consonantForm;
        private readonly string _vowelForm;
        private readonly IEnumerable<string> _exceptions;
        private readonly int _maxExceptionLength; // This is used to speed up searching

        /// <summary>
        /// The consonant form of the current indefinite article.
        /// </summary>
        public string ConsonantForm
        {
            get { return _consonantForm; }
        }

        /// <summary>
        /// The vowel form of the current indefinite article.
        /// </summary>
        public string Vowel
        {
            get { return _vowelForm; }
        }

        public IEnumerable<string> Exceptions
        {
            get { return _exceptions; }
        }

        /// <summary>
        /// Creates a new IndefiniteArticle object with the specified values.
        /// </summary>
        /// <param name="consonantForm">The consonant form of the article.</param>
        /// <param name="vowelForm">The vowel form of the article.</param>
        /// <param name="exceptions">A list of strings that describe word prefixes that invalidate the usage of the vowel form (i.e. "union", "unicorn", "universe")</param>
        public IndefiniteArticle(string consonantForm, string vowelForm, params string[] exceptions)
        {
            _consonantForm = consonantForm ?? "";
            _vowelForm = vowelForm ?? "";
            _exceptions = exceptions.OrderByDescending(str => str.Length);
            _maxExceptionLength = exceptions.Max(str => str.Length);
        }

        internal bool CanPluralize(StringBuilder sb)
        {
            if (sb.Length == 0) return false;
            char c;
            for (int i = 0; i < sb.Length; i++)
            {
                c = sb[i];
                if (Char.IsWhiteSpace(c) || Char.IsSeparator(c)) continue;
                if (Char.IsNumber(c)) return false;
                if (!Char.IsLetter(c)) continue;
                if ("aeiou".IndexOf(c.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase) == -1) return false;

                if (i + _maxExceptionLength > sb.Length) return true;

                var slice = new char[_maxExceptionLength];
                sb.CopyTo(i, slice, 0, _maxExceptionLength);
                var sliceString = new string(slice);
                return _exceptions.All(e => !sliceString.StartsWith(e, StringComparison.InvariantCultureIgnoreCase));
            }
            return false;
        }
    }
}