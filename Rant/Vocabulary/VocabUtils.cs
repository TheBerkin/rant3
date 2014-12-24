using System;
using System.Collections.Generic;
using System.Linq;

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

        public static bool AssociatesWith(this RantDictionaryEntry a, RantDictionaryEntry b)
        {
            if (a == null || b == null) return false;

            bool aNoneRequired = !a.GetRequiredClasses().Any();
            bool bNoneRequired = !b.GetRequiredClasses().Any();

            if (aNoneRequired && bNoneRequired) return true; // If both have no required classes, pass.

            // One or both have required classes.

            // Remove B optionals from A required.
            var aRequired = a.GetRequiredClasses().Except(b.GetOptionalClasses());
            // Remove A optionals from B required.
            var bRequired = b.GetRequiredClasses().Except(a.GetOptionalClasses());

            // Both should be either empty, or have exactly the same classes.
            return !aRequired.Except(bRequired).Any() && aRequired.Any() == bRequired.Any();
        }

        public static bool RelatesWith(this RantDictionaryEntry a, RantDictionaryEntry b)
        {
            if (a == null || b == null) return false;

            bool aNoneRequired = !a.GetRequiredClasses().Any();
            bool bNoneRequired = !b.GetRequiredClasses().Any();

            if (aNoneRequired && bNoneRequired) return true; // If both have no required classes, pass.

            // One or both have required classes.

            // Remove B optionals from A required.
            var aRequired = a.GetRequiredClasses().Except(b.GetOptionalClasses());
            // Remove A optionals from B required.
            var bRequired = b.GetRequiredClasses().Except(a.GetOptionalClasses());

            // Both should share at least one class.
            return aRequired.Intersect(bRequired).Any();
        }

        public static bool DivergesFrom(this RantDictionaryEntry a, RantDictionaryEntry b)
        {
            if (a == null || b == null) return false;

            bool aNoneRequired = !a.GetRequiredClasses().Any();
            bool bNoneRequired = !b.GetRequiredClasses().Any();

            if (aNoneRequired && bNoneRequired) return true; // If both have no required classes, pass.

            // One or both have required classes.

            // Remove B optionals from A required.
            var aRequired = a.GetRequiredClasses().Except(b.GetOptionalClasses());
            // Remove A optionals from B required.
            var bRequired = b.GetRequiredClasses().Except(a.GetOptionalClasses());

            // Both should be either empty, or differ by at least one class.
            return aRequired.Except(bRequired).Any() || bRequired.Except(aRequired).Any();
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