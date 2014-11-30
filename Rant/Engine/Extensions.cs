using System.Runtime.CompilerServices;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Rant
{
    internal static class __
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte RotL(this sbyte data, int times)
        {
            return (sbyte)((data << (times % 8)) | (data >> (8 - (times % 8))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte RotR(this sbyte data, int times)
        {
            return (sbyte)((data >> (times % 8)) | (data << (8 - (times % 8))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte RotL(this byte data, int times)
        {
            return (byte)((data << (times % 8)) | (data >> (8 - (times % 8))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte RotR(this byte data, int times)
        {
            return (byte)((data >> (times % 8)) | (data << (8 - (times % 8))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short RotL(this short data, int times)
        {
            return (short)((data << (times % 16)) | (data >> (16 - (times % 16))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short RotR(this short data, int times)
        {
            return (short)((data >> (times % 16)) | (data << (16 - (times % 16))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RotL(this ushort data, int times)
        {
            return (ushort)((data << (times % 16)) | (data >> (16 - (times % 16))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RotR(this ushort data, int times)
        {
            return (ushort)((data >> (times % 16)) | (data << (16 - (times % 16))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RotL(this int data, int times)
        {
            return (data << (times % 32)) | (data >> (32 - times % 32));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RotR(this int data, int times)
        {
            return (data >> (times % 32)) | (data << (32 - times % 32));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotL(this uint data, int times)
        {
            return (data << (times % 32)) | (data >> (32 - (times % 32)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotR(this uint data, int times)
        {
            return (data >> (times % 32)) | (data << (32 - (times % 32)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RotL(this long data, int times)
        {
            return (data << (times % 64)) | (data >> (64 - (times % 64)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RotR(this long data, int times)
        {
            return (data >> (times % 64)) | (data << (64 - (times % 64)));
        }

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