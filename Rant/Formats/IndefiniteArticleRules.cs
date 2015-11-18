using System;
using System.Linq;

namespace Rant.Formats
{
    /// <summary>
    /// Represents a rule set for determining the correct indefinite article to place before a word.
    /// </summary>
    public sealed class IndefiniteArticleRules
    {
        /// <summary>
        /// Predefined article rules for English.
        /// </summary>
        public static readonly IndefiniteArticleRules English = new IndefiniteArticleRules(
            new[] { "uni", "use", "uri", "urol", "U.", "one", "uvu", "eul", "euk", "eur" },
            new[] { "honest", "honor", "hour", "8" },
            new[] { "u" },
            new[] { "f", "fbi", "fcc", "fda", "x", "l", "m", "n", "s", "h" });

        /// <summary>
        /// Creates a new IndefiniteArticleRules instance with the specified parameters that uses default vowels (a, e, i, o, u).
        /// </summary>
        /// <param name="ignorePrefixes">The word prefixes that are to be excluded from vowel tests.</param>
        /// <param name="allowPrefixes">The word prefixes that are to be included in vowel tests, which would normally test negative.</param>
        /// <param name="ignoreWords">The words that should be ignored in vowel tests.</param>
        /// <param name="allowWords">The words that should be included in vowel tests, which would normally test negative.</param>
        public IndefiniteArticleRules(string[] ignorePrefixes, string[] allowPrefixes, string[] ignoreWords, string[] allowWords)
        {
            IgnorePrefixes = (ignorePrefixes ?? new string[0]);
            AllowPrefixes = (allowPrefixes ?? new string[0]);
            IgnoreWords = (ignoreWords ?? new string[0]);
            AllowWords = (allowWords ?? new string[0]);
        }

        /// <summary>
        /// The vowel characters that the rules should test for.
        /// </summary>
        public char[] Vowels { get; set; } = { 'a', 'e', 'i', 'o', 'u', 'é' };

        /// <summary>
        /// The word prefixes that are to be excluded from vowel tests.
        /// </summary>
        public string[] IgnorePrefixes { get; set; } = new string[0];

        /// <summary>
        /// The word prefixes that are to be included in vowel tests, which would normally test negative.
        /// </summary>
        public string[] AllowPrefixes { get; set; } = new string[0];

        /// <summary>
        /// The words that should be ignored in vowel tests.
        /// </summary>
        public string[] IgnoreWords { get; set; } = new string[0];

        /// <summary>
        /// The words that should be included in vowel tests, which would normally test negative.
        /// </summary>
        public string[] AllowWords { get; set; } = new string[0];

        internal bool Check(string value)
        {
            if (String.IsNullOrEmpty(value)) return false;
            return 
                (AllowWords.Any(word => String.Equals(word, value, StringComparison.InvariantCultureIgnoreCase)) 
                    || AllowPrefixes.Any(pfx => value.StartsWith(pfx, StringComparison.InvariantCultureIgnoreCase)))
                || (Vowels.Any(v => Char.ToUpperInvariant(v) == Char.ToUpperInvariant(value[0]))
                    && !IgnorePrefixes.Any(pfx => value.StartsWith(pfx, StringComparison.InvariantCultureIgnoreCase))
                    && !IgnoreWords.Any(word => String.Equals(word, value, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
