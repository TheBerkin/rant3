using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant
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
            new []{"uni", "use", "uri", "urol", "euro", "U.", "one", "uvu"},
            new []{"honest", "honor", "8"},
            new []{"u"},
            new string[0]);

        private readonly IEnumerable<string> _ignorePrefixes;
        private readonly IEnumerable<string> _allowPrefixes;
        private readonly IEnumerable<string> _ignoreWords;
        private readonly IEnumerable<string> _allowWords;
        private readonly IEnumerable<char> _vowels;

        /// <summary>
        /// Creates a new IndefiniteArticleRules instance with the specified parameters that uses default vowels (a, e, i, o, u).
        /// </summary>
        /// <param name="ignorePrefixes">The word prefixes that are to be excluded from vowel tests.</param>
        /// <param name="allowPrefixes">The word prefixes that are to be included in vowel tests, which would normally test negative.</param>
        /// <param name="ignoreWords">The words that should be ignored in vowel tests.</param>
        /// <param name="allowWords">The words that should be included in vowel tests, which would normally test negative.</param>
        public IndefiniteArticleRules(string[] ignorePrefixes, string[] allowPrefixes, string[] ignoreWords, string[] allowWords)
        {
            _vowels = new[] {'a', 'e', 'i', 'o', 'u'};
            _ignorePrefixes = (ignorePrefixes ?? new string[0]);
            _allowPrefixes = (allowPrefixes ?? new string[0]);
            _ignoreWords = (ignoreWords ?? new string[0]);
            _allowWords = (allowWords ?? new string[0]);
        }

        /// <summary>
        /// Creates a new IndefiniteArticleRules instance with the specified parameters.
        /// </summary>
        /// <param name="vowels">The vowel characters that the rules should test for.</param>
        /// <param name="ignorePrefixes">The word prefixes that are to be excluded from vowel tests.</param>
        /// <param name="allowPrefixes">The word prefixes that are to be included in vowel tests, which would normally test negative.</param>
        /// <param name="ignoreWords">The words that should be ignored in vowel tests.</param>
        /// <param name="allowWords">The words that should be included in vowel tests, which would normally test negative.</param>
        public IndefiniteArticleRules(char[] vowels, string[] ignorePrefixes, string[] allowPrefixes, string[] ignoreWords, string[] allowWords)
        {
            _vowels = vowels ?? new[] {'a', 'e', 'i', 'o', 'u'};
            _ignorePrefixes = (ignorePrefixes ?? new string[0]);
            _allowPrefixes = (allowPrefixes ?? new string[0]);
            _ignoreWords = (ignoreWords ?? new string[0]);
            _allowWords = (allowWords ?? new string[0]);
        }

        /// <summary>
        /// The vowel characters that the rules should test for.
        /// </summary>
        public IEnumerable<char> Vowels => _vowels;

        /// <summary>
        /// The word prefixes that are to be excluded from vowel tests.
        /// </summary>
        public IEnumerable<string> IgnorePrefixes => _ignorePrefixes;

        /// <summary>
        /// The word prefixes that are to be included in vowel tests, which would normally test negative.
        /// </summary>
        public IEnumerable<string> AllowPrefixes => _allowPrefixes;

        /// <summary>
        /// The words that should be ignored in vowel tests.
        /// </summary>
        public IEnumerable<string> IgnoreWords => _ignoreWords;

        /// <summary>
        /// The words that should be included in vowel tests, which would normally test negative.
        /// </summary>
        public IEnumerable<string> AllowWords => _allowWords;

        internal bool Check(string value)
        {
            if (String.IsNullOrEmpty(value)) return false;
            return 
                (_allowWords.Any(word => String.Equals(word, value, StringComparison.InvariantCultureIgnoreCase)) 
                    || _allowPrefixes.Any(pfx => value.StartsWith(pfx, StringComparison.InvariantCultureIgnoreCase)))
                || (_vowels.Any(v => Char.ToUpperInvariant(v) == Char.ToUpperInvariant(value[0]))
                    && !_ignorePrefixes.Any(pfx => value.StartsWith(pfx, StringComparison.InvariantCultureIgnoreCase))
                    && !_ignoreWords.Any(word => String.Equals(word, value, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}