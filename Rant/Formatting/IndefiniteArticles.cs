using System;
using System.Text;

namespace Rant.Formatting
{
    /// <summary>
    /// Defines indefinite article (\a) formatting to use when executing Rant patterns.
    /// </summary>
    public sealed class IndefiniteArticles
    {
        /// <summary>
        /// Indefinite articles for English.
        /// </summary>
        public static readonly IndefiniteArticles English = new IndefiniteArticles("a", "an", IndefiniteArticleRules.English);

        private readonly string _consonantForm;
        private readonly string _vowelForm;
        private readonly IndefiniteArticleRules _rules;

        /// <summary>
        /// The consonant form of the current indefinite article.
        /// </summary>
        public string ConsonantForm => _consonantForm;

        /// <summary>
        /// The vowel form of the current indefinite article.
        /// </summary>
        public string VowelForm => _vowelForm;

        /// <summary>
        /// Creates a new IndefiniteArticle object with the specified values and rules.
        /// </summary>
        /// <param name="consonantForm">The consonant form of the article.</param>
        /// <param name="vowelForm">The vowel form of the article.</param>
        /// <param name="rules"></param>
        public IndefiniteArticles(string consonantForm, string vowelForm, IndefiniteArticleRules rules)
        {
            _rules = rules;
            _consonantForm = consonantForm ?? "";
            _vowelForm = vowelForm ?? "";
        }

        internal bool PrecedesVowel(StringBuilder sb)
        {
            if (sb.Length == 0) return false;
            char c;
            int start = -1;
            int end = 0;
            for (int i = 0; i < sb.Length; i++)
            {
                c = sb[i];
                if (start == -1)
                {
                    if (Char.IsWhiteSpace(c) || Char.IsSeparator(c)) continue; // Must be padding, skip it
                    if (!Char.IsLetterOrDigit(c)) continue;
                    start = i;
                    if (i == sb.Length - 1) end = start + 1; // Word is one character long
                }
                else
                {
                    end = i;
                    if (!Char.IsLetterOrDigit(c)) break;
                    if (i == sb.Length - 1) end++; // Consume character if it's the last one in the buffer
                }
            }

            if (start == -1) return false;

            var buffer = new char[end - start];
            sb.CopyTo(start, buffer, 0, end - start);
            return _rules.Check(new string(buffer));
        }
    }
}