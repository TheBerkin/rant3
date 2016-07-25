using System;
using System.Linq;

using Rant.Metadata;

namespace Rant.Vocabulary.Utilities
{
	internal class Rhymer
	{
		private static readonly char[] _vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
		private static readonly char[] _vowelSounds = { 'A', 'i', 'I', 'E', 'e', '3', '{', 'V', 'O', 'U', 'u', '^' };

		public RhymeFlags AllowedRhymes { get; set; }

		public Rhymer()
		{
			AllowedRhymes = RhymeFlags.Perfect;
		}

		private bool IsEnabled(RhymeFlags flags) => (AllowedRhymes & flags) == flags;

		public bool Rhyme(RantDictionaryTerm term1, RantDictionaryTerm term2)
		{
			bool hasStress = term1.Pronunciation.IndexOf('"') > -1 && term2.Pronunciation.IndexOf('"') > -1;
			// syllables after the stress are the same
			if (IsEnabled(RhymeFlags.Perfect) && hasStress)
			{
				var pron1 = term1.Pronunciation.Substring(term1.Pronunciation.IndexOf('"')).Replace("-", string.Empty);
				var pron2 = term2.Pronunciation.Substring(term2.Pronunciation.IndexOf('"')).Replace("-", string.Empty);
				pron1 = GetFirstVowelSound(pron1);
				pron2 = GetFirstVowelSound(pron2);
				if (pron1 == pron2) return true;
			}
			// last syllables are the same
			if (IsEnabled(RhymeFlags.Syllabic))
			{
				if (term1.Syllables.Last() == term2.Syllables.Last()) return true;
			}
			// penultimate syllable is stressed but does not rhyme, last syllable rhymes
			if (IsEnabled(RhymeFlags.Weak) && hasStress)
			{
				if (
					term1.SyllableCount >= 2 &&
					term2.SyllableCount >= 2 &&
					term1.Syllables[term1.SyllableCount - 2].IndexOf('"') > -1 &&
					term2.Syllables[term2.SyllableCount - 2].IndexOf('"') > -1 &&
					GetFirstVowelSound(term1.Syllables.Last()) == GetFirstVowelSound(term2.Syllables.Last())
				  )
					return true;
			}
			if (IsEnabled(RhymeFlags.Semirhyme))
			{
				if (Math.Abs(term1.SyllableCount - term2.SyllableCount) == 1)
				{
					var longestWord = term1.SyllableCount > term2.SyllableCount ? term1 : term2;
					var shortestWord = term1.SyllableCount > term2.SyllableCount ? term2 : term1;
					if (
						GetFirstVowelSound(longestWord.Syllables[longestWord.SyllableCount - 2]) ==
						GetFirstVowelSound(shortestWord.Syllables.Last()))
						return true;
				}
			}
			// psuedo-sound similar
			if (IsEnabled(RhymeFlags.Forced))
			{
				var distance = LevenshteinDistance(
					term1.Value.GenerateDoubleMetaphone(),
					term2.Value.GenerateDoubleMetaphone()
				);
				if (distance <= 1)
					return true;
			}
			// matching final consonants
			if (IsEnabled(RhymeFlags.SlantRhyme))
			{
				// WE ARE REVERSING THESE STRINGS OK
				string word1 = new string(term1.Value.Reverse().ToArray());
				string word2 = new string(term2.Value.Reverse().ToArray());
				if (GetFirstConsonants(word1) == GetFirstConsonants(word2))
					return true;
			}
			// matching first consonants
			if (IsEnabled(RhymeFlags.Alliteration))
			{
				if (GetFirstConsonants(term1.Value) == GetFirstConsonants(term2.Value))
					return true;
			}
			// matching all consonants
			if (IsEnabled(RhymeFlags.Pararhyme))
			{
				if (term1.Value
					.Where(x => !_vowels.Contains(x))
					.SequenceEqual(
						term2.Value
						.Where(x => !_vowels.Contains(x))
					))
					return true;
			}

			return false;
		}

		public string GetFirstConsonants(string word)
		{
			int i;
			for (i = 0; i < word.Length; i++)
			{
				if (_vowels.Contains(word[i]))
					break;
			}
			if (i > 0)
				return word.Substring(0, i);
			return word;
		}

		// basically, strip consonants from the pronunciation
		public string GetFirstVowelSound(string pron)
		{
			int i;
			for (i = 0; i < pron.Length; i++)
			{
				if (_vowelSounds.Contains(pron[i]))
					break;
			}
			return pron.Substring(i);
		}

		// https://en.wikibooks.org/wiki/Algorithm_Implementation/Strings/Levenshtein_distance#C.23
		public int LevenshteinDistance(string source, string target)
		{
			if (String.IsNullOrEmpty(source))
			{
				if (String.IsNullOrEmpty(target)) return 0;
				return target.Length;
			}
			if (String.IsNullOrEmpty(target)) return source.Length;

			if (source.Length > target.Length)
			{
				var temp = target;
				target = source;
				source = temp;
			}

			var m = target.Length;
			var n = source.Length;
			var distance = new int[2, m + 1];
			// Initialize the distance 'matrix'
			for (var j = 1; j <= m; j++) distance[0, j] = j;

			var currentRow = 0;
			for (var i = 1; i <= n; ++i)
			{
				currentRow = i & 1;
				distance[currentRow, 0] = i;
				var previousRow = currentRow ^ 1;
				for (var j = 1; j <= m; j++)
				{
					var cost = (target[j - 1] == source[i - 1] ? 0 : 1);
					distance[currentRow, j] = Math.Min(Math.Min(
								distance[previousRow, j] + 1,
								distance[currentRow, j - 1] + 1),
								distance[previousRow, j - 1] + cost);
				}
			}
			return distance[currentRow, m];
		}
	}

	[Flags]
	internal enum RhymeFlags : byte
	{
		[RantDescription("Everything after the first stressed vowel matches in pronunciation (picky / icky).")]
		Perfect = 0x01,
		[RantDescription("The penultimate syllable is stressed and the final syllable rhymes (coffin / raisin).")]
		Weak = 0x02,
		[RantDescription("The final syllable rhymes (senator / otter).")]
		Syllabic = 0x04,
		[RantDescription("The words would rhyme if not for the final syllable (broom / broomstick).")]
		Semirhyme = 0x08,
		[RantDescription("The words might rhyme if you really pushed it.")]
		Forced = 0x10,
		[RantDescription("The ending consonants are the same (rant / ant).")]
		SlantRhyme = 0x20,
		[RantDescription("All the consonants match (tuna / teen).")]
		Pararhyme = 0x40,
		[RantDescription("All consonants up to the first vowel rhyme (dog / dude).")]
		Alliteration = 0x80
	}
}
