using System;

using Rant.Core.Utilities;

using static Rant.Localization.Txtres;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Represents a single term of a dictionary entry.
	/// </summary>
	public sealed class RantDictionaryTerm
	{
		private string _pronunciation = string.Empty;
		private int _syllableCount;
		private string[] _syllables;
		private string _value;

		/// <summary>
		/// Intializes a new instance of the <see cref="RantDictionaryTerm" /> class with the specified value string.
		/// </summary>
		/// <param name="value">The value of the term.</param>
		/// <param name="splitIndex">The split index of the term value. Specify -1 for no split.</param>
		public RantDictionaryTerm(string value, int splitIndex = -1)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			ValueSplitIndex = splitIndex;
			_value = string.Intern(value);
		}

		/// <summary>
		/// Intializes a new instance of the <see cref="RantDictionaryTerm" /> class with the specified value and pronunciation
		/// strings.
		/// </summary>
		/// <param name="value">The value of the term.</param>
		/// <param name="pronunciation">The pronunciation of the term value.</param>
		public RantDictionaryTerm(string value, string pronunciation)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			_value = string.Intern(value);
			_pronunciation = string.Intern(pronunciation ?? string.Empty);
		}

		/// <summary>
		/// Intializes a new instance of the <see cref="RantDictionaryTerm" /> class with the specified value, pronunciation, and
		/// split indices.
		/// </summary>
		/// <param name="value">The value of the term.</param>
		/// <param name="pronunciation">The pronunciation of the term value.</param>
		/// <param name="valueSplitIndex">The split index of the term value. Specify -1 for no split.</param>
		/// <param name="pronSplitIndex">
		/// The split index of the term pronunciation string. Specify -1 for no split. Must be
		/// positive if the value is split and pronunciation data is present.
		/// </param>
		public RantDictionaryTerm(string value, string pronunciation, int valueSplitIndex, int pronSplitIndex)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (valueSplitIndex < 0 != pronSplitIndex < 0 && !Util.IsNullOrWhiteSpace(pronunciation)) throw new ArgumentException(GetString("err-incomplete-term-split"));
			if (valueSplitIndex > value.Length)
				throw new ArgumentException(GetString("err-invalid-term-split"), nameof(valueSplitIndex));
			if (pronSplitIndex > pronunciation?.Length)
				throw new ArgumentException(GetString("err-invalid-term-split"), nameof(pronSplitIndex));

			_value = string.Intern(value);
			_pronunciation = string.Intern(pronunciation ?? string.Empty);
			ValueSplitIndex = valueSplitIndex;
			PronunciationSplitIndex = pronSplitIndex;
		}

		/// <summary>
		/// The value string of the term.
		/// </summary>
		public string Value
		{
			get { return _value; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				_value = string.Intern(value);
			}
		}

		/// <summary>
		/// Determines whether the term is a split word.
		/// </summary>
		public bool IsSplit => ValueSplitIndex > -1;

		/// <summary>
		/// Gets the split index of the term value.
		/// </summary>
		public int ValueSplitIndex { get; set; } = -1;

		/// <summary>
		/// Gets the split index of the term pronunciation string.
		/// </summary>
		public int PronunciationSplitIndex { get; set; } = -1;

		/// <summary>
		/// The pronunciation of the term.
		/// </summary>
		public string Pronunciation
		{
			get { return _pronunciation; }
			set
			{
				_pronunciation = string.Intern(value ?? string.Empty);
				if (_syllables != null) CreateSyllables();
			}
		}

		/// <summary>
		/// An array containing the individual syllables of the pronunciation string.
		/// </summary>
		public string[] Syllables => _syllables ?? CreateSyllables();

		/// <summary>
		/// The number of syllables in the pronunciation string.
		/// </summary>
		public int SyllableCount
		{
			get
			{
				if (_syllables == null) CreateSyllables();
				return _syllableCount;
			}
		}

		private string[] CreateSyllables()
		{
			_syllables = _pronunciation.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
			_syllableCount = _syllables.Length;
			return _syllables;
		}
	}
}