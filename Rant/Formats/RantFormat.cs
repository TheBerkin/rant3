using System;
using System.Collections.Generic;
using System.Globalization;

using Rant.Engine;

namespace Rant.Formats
{
    /// <summary>
    /// Describes language-specific formatting instructions for localizing interpreter output.
    /// </summary>
    public class RantFormat
    {
        /// <summary>
        /// English formatting.
        /// </summary>
        public static RantFormat English;

        static RantFormat()
        {
            English = new RantFormat
            {
                IndefiniteArticles = IndefiniteArticles.English
            };
            English.TitleCaseExclusions.AddRange(
                "a", "an", "the", "that", "where", "when", "for", "any", "or", "and", "of", "in", "at", "as", "into", "if",
                "are", "you", "why", "from");
        }

	    internal RantFormat()
	    {   
	    }

        private readonly HashSet<string> _titleCaseExcludedWords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Gets the collection of words excluded from Title Case capitalization.
		/// </summary>
	    protected HashSet<string> TitleCaseExclusions => _titleCaseExcludedWords; 

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
        public char[] Letters { get; protected set; } =
		{
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

		/// <summary>
		/// The standard space character used in automated formatting, such as series.
		/// </summary>
	    public char StandardSpace { get; protected set; } = ' ';

        /// <summary>
        /// The vowel-sensitive indefinite articles used by the \a escape sequence.
        /// </summary>
        public IndefiniteArticles IndefiniteArticles { get; protected set; } = IndefiniteArticles.English;

	    /// <summary>
	    /// The culture to format output strings with.
	    /// </summary>
	    public CultureInfo Culture { get; protected set; } = CultureInfo.InvariantCulture;

        internal bool Excludes(string word) => _titleCaseExcludedWords.Contains(word);
    }
}