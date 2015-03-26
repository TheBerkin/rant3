using System;
using System.Linq;

using Rant.Engine;
using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Used to filter query results by syllable count.
    /// </summary>
    /// <param name="syllables">The number of syllables.</param>
    /// <returns></returns>
    public delegate bool SyllablePredicateFunc(int syllables);

    internal static class SyllablePredicate
    {
        public static SyllablePredicateFunc Create(Token<R> rangeToken)
        {
            if (rangeToken.ID != R.RangeLiteral) return null;
            var literal = rangeToken.Value.Trim();
            var range = literal.Substring(1, literal.Length - 2).Split('-').Select(str => str.Trim()).ToArray();
            if (range.Length == 1)
            {
                int num = Int32.Parse(range[0]);
                return x => x == num;
            }
            else if (Util.IsNullOrWhiteSpace(range[0])) // Max
            {
                int num = Int32.Parse(range[1]);
                return x => x <= num;
            }
            else if (Util.IsNullOrWhiteSpace(range[1])) // Min
            {
                int num = Int32.Parse(range[0]);
                return x => x >= num;
            }
            else
            {
                int a = Int32.Parse(range[0]);
                int b = Int32.Parse(range[1]);
                return x => x >= a && x <= b;
            }
        }
    }
}
