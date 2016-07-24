using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Rant.Internals.Engine.Utilities;

namespace Rant.Formats
{
    /// <summary>
    /// Describes language-specific formatting instructions for localizing interpreter output.
    /// </summary>
    public sealed class RantFormat
    {
        /// <summary>
        /// English formatting.
        /// </summary>
        public static RantFormat English;

        static RantFormat()
        {
	        English = new RantFormat();
            English.TitleCaseExclusions.AddRange(
                "a", "an", "the", "that", "where", "when", "for", "any", "or", "and", "of", "in", "at", "as", "into", "if",
                "are", "you", "why", "from");
        }

	    public RantFormat()
	    {   
	    }

	    public RantFormat(CultureInfo culture)
	    {
		    Culture = culture;
	    }

	    public RantFormat(CultureInfo culture, char stdSpace, IEnumerable<char> letters)
	    {
		    Culture = culture;
		    StandardSpace = stdSpace;
		    LettersInternal = letters.ToArray();
	    }

		public RantFormat(CultureInfo culture, char stdSpace, IEnumerable<char> letters,
			char openingPrimaryQuote, char closingPrimaryQuote, char openingSecondaryQuote, char closingSecondaryQuote)
		{
			Culture = culture;
			StandardSpace = stdSpace;
			LettersInternal = letters.ToArray();
			OpeningPrimaryQuote = openingPrimaryQuote;
			ClosingPrimaryQuote = closingPrimaryQuote;
			OpeningSecondaryQuote = openingSecondaryQuote;
			ClosingSecondaryQuote = closingSecondaryQuote;
		}

		public RantFormat(CultureInfo culture, IEnumerable<string> titleCaseExclusions)
		{
			Culture = culture;
			foreach (var word in titleCaseExclusions) _titleCaseExcludedWords.Add(word);
		}

		public RantFormat(CultureInfo culture, IEnumerable<string> titleCaseExclusions, Pluralizer pluralizer)
		{
			Culture = culture;
			foreach (var word in titleCaseExclusions) _titleCaseExcludedWords.Add(word);
			Pluralizer = pluralizer;
		}

		public RantFormat(CultureInfo culture, char stdSpace, IEnumerable<char> letters,
			char openingPrimaryQuote, char closingPrimaryQuote, char openingSecondaryQuote, char closingSecondaryQuote,
            IEnumerable<string> titleCaseExclusions)
		{
			Culture = culture;
			StandardSpace = stdSpace;
			LettersInternal = letters.ToArray();
			OpeningPrimaryQuote = openingPrimaryQuote;
			ClosingPrimaryQuote = closingPrimaryQuote;
			OpeningSecondaryQuote = openingSecondaryQuote;
			ClosingSecondaryQuote = closingSecondaryQuote;
			foreach (var word in titleCaseExclusions) _titleCaseExcludedWords.Add(word);
		}

		public RantFormat(CultureInfo culture, char stdSpace, IEnumerable<char> letters,
			char openingPrimaryQuote, char closingPrimaryQuote, char openingSecondaryQuote, char closingSecondaryQuote,
			IEnumerable<string> titleCaseExclusions, Pluralizer pluralizer)
		{
			Culture = culture;
			StandardSpace = stdSpace;
			LettersInternal = letters.ToArray();
			OpeningPrimaryQuote = openingPrimaryQuote;
			ClosingPrimaryQuote = closingPrimaryQuote;
			OpeningSecondaryQuote = openingSecondaryQuote;
			ClosingSecondaryQuote = closingSecondaryQuote;
			foreach (var word in titleCaseExclusions) _titleCaseExcludedWords.Add(word);
			Pluralizer = pluralizer;
		}

		private readonly HashSet<string> _titleCaseExcludedWords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Gets the collection of words excluded from Title Case capitalization.
		/// </summary>
	    private HashSet<string> TitleCaseExclusions => _titleCaseExcludedWords; 

        #region Quotation marks

        /// <summary>
        /// The opening primary quotation mark.
        /// </summary>
        public char OpeningPrimaryQuote { get; } = '\u201c';
        /// <summary>
        /// The closing primary quotation mark.
        /// </summary>
        public char ClosingPrimaryQuote { get; } = '\u201d';

        /// <summary>
        /// The opening secondary quotation mark.
        /// </summary>
        public char OpeningSecondaryQuote { get; } = '\u2018';
        /// <summary>
        /// The closing secondary quotation mark.
        /// </summary>
        public char ClosingSecondaryQuote { get; } = '\u2019';

        #endregion

	    /// <summary>
	    /// The letter set used by escape sequences like \c and \w.
	    /// </summary>
	    public IEnumerable<char> Letters => LettersInternal.AsEnumerable();

        internal char[] LettersInternal { get; } =
		{
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

		/// <summary>
		/// The standard space character used in automated formatting, such as series.
		/// </summary>
	    public char StandardSpace { get; } = ' ';

	    /// <summary>
	    /// The culture to format output strings with.
	    /// </summary>
	    public CultureInfo Culture { get; } = CultureInfo.InvariantCulture;

		/// <summary>
		/// The pluralizer used by the [plural] function to infer plural nouns.
		/// </summary>
		public Pluralizer Pluralizer { get; } = new EnglishPluralizer();

        internal bool Excludes(string word) => _titleCaseExcludedWords.Contains(word);
    }
}