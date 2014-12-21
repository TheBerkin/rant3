using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rant.Vocabulary
{
    internal static class VocabUtils
    {
        private static readonly Dictionary<string, string> StringCache = new Dictionary<string, string>();

        public static string GetString(string str)
        {
            string cstr;
            if (!StringCache.TryGetValue(str, out cstr)) cstr = StringCache[str] = str;
            return cstr;
        }

        public static int RhymeIndex(RantDictionaryTerm baseValue, RantDictionaryTerm testValue)
        {
            if (baseValue == null || testValue == null) return 0;
            var baseParts = baseValue.PronunciationParts;
            var testParts = testValue.PronunciationParts;
            int index = 0;
            int len = Math.Min(baseParts.Length, testParts.Length);
            for (int i = 0; i < len; i++)
            {
                if (baseParts[baseParts.Length - (i + 1)] == testParts[testParts.Length - (i + 1)])
                {
                    index++;
                }
                else
                {
                    return index;
                }
            }
            return index;
        }
    }
}