using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rant.Vocabulary
{
    internal static class VocabUtils
    {
        private static readonly Dictionary<string, string> StringCache = new Dictionary<string, string>();

        public static RantDictionaryEntry PickEntry(this IEnumerable<RantDictionaryEntry> enumerable, RNG rng)
        {
            var array = enumerable as RantDictionaryEntry[] ?? enumerable.ToArray();
            return array.Length > 0 ? array[rng.Next(array.Length)] : null;
        }

        public static IEnumerable<string> GetArgs(string argString)
        {
            if (argString.Length == 0) yield break;
            bool escape = false;
            bool scope = false;
            bool scopeUsed = false;
            int length = argString.Length;
            int i = 0;
            var sb = new StringBuilder();
            do
            {
                if (i >= length || (Char.IsWhiteSpace(argString[i]) && !scope))
                {
                    if (sb.Length == 0 && !scopeUsed) continue;
                    yield return sb.ToString();
                    sb.Length = 0;
                    scopeUsed = false;
                }
                else if (!escape && argString[i] == '\\')
                {
                    escape = true;
                }
                else if (escape)
                {
                    sb.Append(argString[i]);
                    escape = false;
                }
                else if (argString[i] == '"')
                {
                    scope = !scope;
                    scopeUsed = true;
                }
                else
                {
                    sb.Append(argString[i]);
                }
            } while (i++ <= length);
        } 

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
    }
}