using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant
{
    public sealed class IndefiniteArticleRules
    {
        public static readonly IndefiniteArticleRules English = new IndefiniteArticleRules(
            new []{"uni", "use", "uri", "urol", "euro", "U.", "one", "uvu", "8"},
            new []{"honest", "honor"},
            new []{"u"},
            new string[0]);

        private readonly IEnumerable<string> _ignorePrefixes;
        private readonly IEnumerable<string> _allowPrefixes;
        private readonly IEnumerable<string> _ignoreWords;
        private readonly IEnumerable<string> _allowWords;
        private readonly IEnumerable<char> _vowels;

        public IndefiniteArticleRules(string[] ignorePrefixes, string[] allowPrefixes, string[] ignoreWords, string[] allowWords)
        {
            _vowels = new[] {'a', 'e', 'i', 'o', 'u'};
            _ignorePrefixes = (ignorePrefixes ?? new string[0]);
            _allowPrefixes = (allowPrefixes ?? new string[0]);
            _ignoreWords = (ignoreWords ?? new string[0]);
            _allowWords = (allowWords ?? new string[0]);
        }

        public IndefiniteArticleRules(char[] vowels, string[] ignorePrefixes, string[] allowPrefixes, string[] ignoreWords, string[] allowWords)
        {
            _vowels = vowels ?? new[] {'a', 'e', 'i', 'o', 'u'};
            _ignorePrefixes = (ignorePrefixes ?? new string[0]);
            _allowPrefixes = (allowPrefixes ?? new string[0]);
            _ignoreWords = (ignoreWords ?? new string[0]);
            _allowWords = (allowWords ?? new string[0]);
        }

        public IEnumerable<char> Vowels
        {
            get { return _vowels; }
        }

        public IEnumerable<string> IgnorePrefixes
        {
            get { return _ignorePrefixes; }
        }

        public IEnumerable<string> AllowPrefixes
        {
            get { return _allowPrefixes; }
        }

        public IEnumerable<string> IgnoreWords
        {
            get { return _ignoreWords; }
        }

        public IEnumerable<string> AllowWords
        {
            get { return _allowWords; }
        }

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