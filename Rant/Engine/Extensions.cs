using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Rant.Engine
{
    internal static class Extensions
    {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotL(this ulong data, int times)
        {
            return (data << (times % 64)) | (data >> (64 - (times % 64)));
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public static T PickWeighted<T>(this IEnumerable<T> collection, RNG rng, Func<T, int> weightSelectionFunc, T defaultValue = default(T))
        {
            int selection = rng.Next(collection.Sum(weightSelectionFunc));

            foreach (T t in collection)
            {
                if (selection < weightSelectionFunc(t))
                {
                    return t;
                }
                selection -= weightSelectionFunc(t);
            }
            return defaultValue;
        }

        public static T PickWeighted<T>(this IEnumerable<T> collection, RNG rng, Func<T, int> weightSelectionFunc, Func<RNG, int, int> rngSelectionFunc, T defaultValue = default(T))
        {
            int selection = rngSelectionFunc(rng, collection.Sum(weightSelectionFunc));

            foreach (T t in collection)
            {
                if (selection < weightSelectionFunc(t))
                {
                    return t;
                }
                selection -= weightSelectionFunc(t);
            }
            return defaultValue;
        }

        public static T PickWeighted<T>(this IEnumerable<T> collection, RNG rng, Func<T, double> weightSelectionFunc, T defaultValue = default(T))
        {
            double selection = rng.NextDouble(collection.Sum(weightSelectionFunc));

            foreach (T t in collection)
            {
                if (selection < weightSelectionFunc(t))
                {
                    return t;
                }
                selection -= weightSelectionFunc(t);
            }
            return defaultValue;
        }

        public static T PickWeighted<T>(this IEnumerable<T> collection, RNG rng, double totalWeight, Func<T, double> weightSelectionFunc, T defaultValue = default(T))
        {
            double selection = rng.NextDouble(totalWeight);

            foreach (T t in collection)
            {
                if (selection < weightSelectionFunc(t))
                {
                    return t;
                }
                selection -= weightSelectionFunc(t);
            }
            return defaultValue;
        }
    }
}