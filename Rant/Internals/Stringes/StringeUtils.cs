using System;
using System.Linq;

namespace Rant.Internals.Stringes
{
    internal static class StringeUtils
    {
        public static int HashOf(params object[] objects)
        {
            return unchecked(objects.Select(o => o.GetHashCode()).Aggregate(17, (hash, next) => hash * 31 + next));
        }

        public static int GetMatchCount(string parent, string sub)
        {
            if (parent == null || sub == null) return 0;
            if (parent.Length * sub.Length == 0) return 0;
            int next = 0;
            int start = 0;
            int count = 0;
            while (start + sub.Length < parent.Length)
            {
                if ((next = parent.IndexOf(sub, start, StringComparison.InvariantCulture)) == -1) return count;
                start = next + sub.Length;
                count++;
            }
            return count;
        }
    }
}