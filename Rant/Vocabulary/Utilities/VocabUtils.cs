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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rant.Vocabulary.Utilities
{
    internal static class VocabUtils
    {
        private static readonly Dictionary<string, string> StringCache = new Dictionary<string, string>();

        public static RantDictionaryEntry PickEntry(this List<RantDictionaryEntry> list, RNG rng, bool useWeights)
        {
			if (useWeights)
			{
				float sum = list.Sum(e => e.Weight);
				float n = (float)rng.NextDouble(sum);
				RantDictionaryEntry entry;
				for(int i = 0; i < list.Count; i++)
				{
					entry = list[i];
					if (n < entry.Weight)
					{
						return entry;
					}
					n -= entry.Weight;
				}
			}
            return list.Any() ? list[rng.Next(list.Count)] : null;
        }

        public static RantDictionaryEntry PickEntry(this IEnumerable<RantDictionaryEntry> entries, RNG rng, bool useWeights)
        {
			if (useWeights)
			{
				float sum = entries.Sum(e => e.Weight);
				float n = (float)rng.NextDouble(sum);
				foreach(RantDictionaryEntry entry in entries)
				{
					if (n < entry.Weight)
					{
						return entry;
					}
					n -= entry.Weight;
				}
			}
			var array = entries as RantDictionaryEntry[] ?? entries.ToArray();
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
                if (i >= length || char.IsWhiteSpace(argString[i]) && !scope)
                {
                    if (sb.Length == 0 && !scopeUsed) continue;
                    yield return sb.ToString();
                    sb.Length = 0;
                    scopeUsed = false;
                }
                else if (!escape && argString[i] == '\\')
                    escape = true;
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
                    sb.Append(argString[i]);
            } while (i++ <= length);
        }

        public static string GetString(string str)
        {
            if (!StringCache.TryGetValue(str, out string cstr)) cstr = StringCache[str] = str;
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