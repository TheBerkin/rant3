using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Rant.Core.Utilities;

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

		/// <summary>
		/// Creates a new RantFormat instance with default values.
		/// </summary>
		public RantFormat()
		{
		}

		/// <summary>
		/// Creates a new RantFormat instance with the specified culture.
		/// </summary>
		/// <param name="culture">The culture to associate with the format.</param>
		public RantFormat(CultureInfo culture)
		{
			Culture = culture;
		}

		/// <summary>
		/// Creates a new RantFormat instance with the specified culture, standard space, and alphabet.
		/// </summary>
		/// <param name="culture">The culture to associate with the format.</param>
		/// <param name="stdSpace">The standard space character to use.</param>
		/// <param name="letters">The alphabet to use with the format.</param>
		public RantFormat(CultureInfo culture, char stdSpace, IEnumerable<char> letters)
		{
			Culture = culture;
			StandardSpace = stdSpace;
			LettersInternal = letters.ToArray();
		}

		/// <summary>
		/// Creates a new RantFormat instance with the specified culture, standard space, alphabet, and quotation marks.
		/// </summary>
		/// <param name="culture">The culture to associate with the format.</param>
		/// <param name="stdSpace">The standard space character to use.</param>
		/// <param name="letters">The alphabet to use with the format.</param>
		/// <param name="openingPrimaryQuote">The opening primary quotation mark to use.</param>
		/// <param name="closingPrimaryQuote">The closing primary quotation mark to use.</param>
		/// <param name="openingSecondaryQuote">The opening secondary quotation mark to use.</param>
		/// <param name="closingSecondaryQuote">The closing secondary quotation mark to use.</param>
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

		/// <summary>
		/// Creates a new RantFormat instance with the specified culture and title case exclusion list.
		/// </summary>
		/// <param name="culture">The culture to associate with the format.</param>
		/// <param name="titleCaseExclusions">A collection of words to exclude from title case capitalization.</param>
		public RantFormat(CultureInfo culture, IEnumerable<string> titleCaseExclusions)
		{
			Culture = culture;
			foreach (string word in titleCaseExclusions) TitleCaseExclusions.Add(word);
		}

		/// <summary>
		/// Creates a new RantFormat instance with the specified culture, title case exclusion list, and pluralizer.
		/// </summary>
		/// <param name="culture">The culture to associate with the format.</param>
		/// <param name="titleCaseExclusions">A collection of words to exclude from title case capitalization.</param>
		/// <param name="pluralizer">The pluralizer to use.</param>
		public RantFormat(CultureInfo culture, IEnumerable<string> titleCaseExclusions, Pluralizer pluralizer)
		{
			Culture = culture;
			foreach (string word in titleCaseExclusions) TitleCaseExclusions.Add(word);
			Pluralizer = pluralizer;
		}

		/// <summary>
		/// Creates a new RantFormat instance with the specified culture, standard space, alphabet, quotation marks, and title case exclusion list.
		/// </summary>
		/// <param name="culture">The culture to associate with the format.</param>
		/// <param name="stdSpace">The standard space character to use.</param>
		/// <param name="letters">The alphabet to use with the format.</param>
		/// <param name="openingPrimaryQuote">The opening primary quotation mark to use.</param>
		/// <param name="closingPrimaryQuote">The closing primary quotation mark to use.</param>
		/// <param name="openingSecondaryQuote">The opening secondary quotation mark to use.</param>
		/// <param name="closingSecondaryQuote">The closing secondary quotation mark to use.</param>
		/// <param name="titleCaseExclusions">A collection of words to exclude from title case capitalization.</param>
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
			foreach (string word in titleCaseExclusions) TitleCaseExclusions.Add(word);
		}

		/// <summary>
		/// Creates a new RantFormat instance with the specified culture, standard space, alphabet, quotation marks, title case exclusion list, and pluralizer.
		/// </summary>
		/// <param name="culture">The culture to associate with the format.</param>
		/// <param name="stdSpace">The standard space character to use.</param>
		/// <param name="letters">The alphabet to use with the format.</param>
		/// <param name="openingPrimaryQuote">The opening primary quotation mark to use.</param>
		/// <param name="closingPrimaryQuote">The closing primary quotation mark to use.</param>
		/// <param name="openingSecondaryQuote">The opening secondary quotation mark to use.</param>
		/// <param name="closingSecondaryQuote">The closing secondary quotation mark to use.</param>
		/// <param name="titleCaseExclusions">A collection of words to exclude from title case capitalization.</param>
		/// <param name="pluralizer">The pluralizer to use.</param>
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
			foreach (string word in titleCaseExclusions) TitleCaseExclusions.Add(word);
			Pluralizer = pluralizer;
		}

		/// <summary>
		/// Gets the collection of words excluded from Title Case capitalization.
		/// </summary>
		private HashSet<string> TitleCaseExclusions { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

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

		internal bool Excludes(string word) => TitleCaseExclusions.Contains(word);

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
	}
}