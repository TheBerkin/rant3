using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rant.Formats
{
    /// <summary>
    /// Describes language-specific formatting instructions for localizing interpreter output.
    /// </summary>
    public abstract class RantFormat
    {
        /// <summary>
        /// English formatting.
        /// </summary>
        public static RantFormat English;

        static RantFormat()
        {
            English = new RantSystemFormat
            {
                IndefiniteArticles = IndefiniteArticles.English
            };
            English.InternalAddTitleCaseExclusions(
                "a", "an", "the", "that", "where", "when", "for", "any", "or", "and", "of", "in", "at", "as", "into", "if",
                "are", "you", "why", "from");
        }

        private readonly HashSet<string> _titleCaseExcludedWords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase); 

        #region Quotation marks

        /// <summary>
        /// The opening primary quotation mark.
        /// </summary>
        public char OpeningPrimaryQuote { get; protected set; } = '\u201c';
        /// <summary>
        /// The closing primary quotation mark.
        /// </summary>
        public char ClosingPrimaryQuote { get; protected set; } = '\u201d';

        /// <summary>
        /// The opening secondary quotation mark.
        /// </summary>
        public char OpeningSecondaryQuote { get; protected set; } = '\u2018';
        /// <summary>
        /// The closing secondary quotation mark.
        /// </summary>
        public char ClosingSecondaryQuote { get; protected set; } = '\u2019';

        #endregion

        /// <summary>
        /// The letter set used by escape sequences like \c and \w.
        /// </summary>
        public char[] Letters { get; protected set; } = new []
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        /// <summary>
        /// The vowel-sensitive indefinite articles used by the \a escape sequence.
        /// </summary>
        public IndefiniteArticles IndefiniteArticles { get; protected set; } = IndefiniteArticles.English;

		/// <summary>
		/// The culture to format output strings with.
		/// </summary>
		public CultureInfo Culture { get; protected set; } = CultureInfo.InvariantCulture;

        protected void InternalAddTitleCaseExclusions(params string[] words)
        {
            foreach (var word in words) _titleCaseExcludedWords.Add(word);
        }

        /// <summary>
        /// Adds the specified strings to the title case exclusion list for the current format.
        /// </summary>
        /// <param name="words">The words to exclude from title case capitalization.</param>
        public virtual void AddTitleCaseExclusions(params string[] words) => InternalAddTitleCaseExclusions(words);

        internal bool Excludes(string word) => _titleCaseExcludedWords.Contains(word);
    }
}