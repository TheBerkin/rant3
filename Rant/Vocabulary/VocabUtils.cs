using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary
{
    internal static class VocabUtils
    {
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