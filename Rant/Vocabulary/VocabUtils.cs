using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rant.Vocabulary
{
    internal static class VocabUtils
    {
        public static void SortClass(string className, HashSet<string> classes, HashSet<string> optionals, Dictionary<string, string> classTable)
        {
            if (className.EndsWith("?"))
            {
                optionals.Add(GetClass(classTable, className.TrimEnd('?')));
            }
            else
            {
                classes.Add(GetClass(classTable, className));
            }
        }

        public static void SortClass(string className, HashSet<string> classes, HashSet<string> optionals)
        {
            if (className.EndsWith("?"))
            {
                optionals.Add(className.TrimEnd('?'));
            }
            else
            {
                classes.Add(className);
            }
        }

        // This saves memory by reusing references to common class names.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetClass(Dictionary<string, string> classTable, string className)
        {
            string c;
            if (!classTable.TryGetValue(className, out c))
            {
                classTable[className] = c = className;
            }
            return c;
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