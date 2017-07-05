#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Linq;

namespace Rant.Vocabulary.Utilities
{
    internal class Rhymer
    {
        private static readonly char[] _vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
        private static readonly char[] _vowelSounds = { 'A', 'i', 'I', 'E', 'e', '3', '{', 'V', 'O', 'U', 'u', '^' };

        public Rhymer()
        {
            AllowedRhymes = RhymeFlags.Perfect;
        }

        public RhymeFlags AllowedRhymes { get; set; }
        private bool IsEnabled(RhymeFlags flags) => (AllowedRhymes & flags) == flags;

        public bool Rhyme(RantDictionaryTerm term1, RantDictionaryTerm term2)
        {
            bool hasStress = term1.Pronunciation.IndexOf('"') > -1 && term2.Pronunciation.IndexOf('"') > -1;
            // syllables after the stress are the same
            if (IsEnabled(RhymeFlags.Perfect) && hasStress)
            {
                string pron1 = term1.Pronunciation.Substring(term1.Pronunciation.IndexOf('"')).Replace("-", string.Empty);
                string pron2 = term2.Pronunciation.Substring(term2.Pronunciation.IndexOf('"')).Replace("-", string.Empty);
                pron1 = GetFirstVowelSound(pron1);
                pron2 = GetFirstVowelSound(pron2);
                if (pron1 == pron2) return true;
            }
            // last syllables are the same
            if (IsEnabled(RhymeFlags.Syllabic))
                if (term1.Syllables.Last() == term2.Syllables.Last()) return true;
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
                int distance = LevenshteinDistance(
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
            if (string.IsNullOrEmpty(source))
            {
                if (string.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if (string.IsNullOrEmpty(target)) return source.Length;

            if (source.Length > target.Length)
            {
                string temp = target;
                target = source;
                source = temp;
            }

            int m = target.Length;
            int n = source.Length;
            var distance = new int[2, m + 1];
            // Initialize the distance 'matrix'
            for (int j = 1; j <= m; j++) distance[0, j] = j;

            int currentRow = 0;
            for (int i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                int previousRow = currentRow ^ 1;
                for (int j = 1; j <= m; j++)
                {
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] = Math.Min(Math.Min(
                            distance[previousRow, j] + 1,
                            distance[currentRow, j - 1] + 1),
                        distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }
    }
}