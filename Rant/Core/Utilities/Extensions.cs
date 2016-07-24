using System.Collections.Generic;

namespace Rant.Core.Utilities
{
    internal static class Extensions
    {
        public static ulong RotL(this ulong data, int times)
        {
            return (data << (times % 64)) | (data >> (64 - (times % 64)));
        }
        
		public static ulong RotR(this ulong data, int times)
        {
            return (data >> (times % 64)) | (data << (64 - (times % 64)));
        }
        
        public static long Hash(this string input)
        {
            unchecked
            {
                long seed = 13;
                foreach (char c in input)
                {
                    seed += c * 19;
                    seed *= 6364136223846793005;
                }
                return seed;
            }
        }

	    public static void AddRange<T>(this HashSet<T> hashset, params T[] items)
	    {
		    foreach (var item in items) hashset.Add(item);
	    }
    }
}