using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rant
{
    /// <summary>
    /// Represents a non-linear random number generator.
    /// </summary>
    public class RNG
    {
        private readonly List<SG> _sg;

        /// <summary>
        /// The current seed.
        /// </summary>
        public long Seed
        {
            get { return _sg[_sg.Count - 1].Seed; }
            set { _sg[_sg.Count - 1].Seed = value; }
        }

        /// <summary>
        /// The current generation.
        /// </summary>
        public long Generation
        {
            get { return _sg[_sg.Count - 1].Generation; }
            set { _sg[_sg.Count - 1].Generation = value; }
        }

        // ReSharper disable once InconsistentNaming
        private class SG
        {
            public long Seed, Generation;

            public SG(long s, long g)
            {
                Seed = s;
                Generation = g;
            }
        }

        /// <summary>
        /// Creates a new ManRandom instance with the specified seed.
        /// </summary>
        /// <param name="seed">The seed for the generator.</param>
        public RNG(long seed)
        {
            _sg = new List<SG> { new SG(seed, 0) };
        }

        /// <summary>
        /// Creates a new ManRandom instance with the specified seed and generation.
        /// </summary>
        /// <param name="seed">The seed for the generator.</param>
        /// <param name="generation">The generation to start at.</param>
        public RNG(long seed, long generation)
        {
            _sg = new List<SG> {new SG(seed, generation)};
        }

        /// <summary>
        /// Creates a new ManRandom instance seeded with the system tick count.
        /// </summary>
        public RNG()
        {
            _sg = new List<SG> { new SG(Environment.TickCount, 0) };
        }

        /// <summary>
        /// Calculates the raw 64-bit value for a given seed/generation pair.
        /// </summary>
        /// <param name="s">The seed.</param>
        /// <param name="g">The generation.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetRaw(long s, long g)
        {
            unchecked
            {
                var v = 6364136223846793005;
                var ss = (s + 113) * 982451653 + 12345;
                var gg = (g + 119) * 32416189717 + 98772341;
                v *= ss.RotR((int)gg) ^ gg.RotL((int)ss);
                v += ss + gg;
                v ^= ss * gg;
                return v;
            }
        }

        /// <summary>
        /// Calculates the raw 64-bit value for a given generation.
        /// </summary>
        /// <param name="g">The generation.</param>
        /// <returns></returns>
        public long this[int g]
        {
            get
            {
                return GetRaw(Seed, g);
            }
        }

        /// <summary>
        /// Calculates the raw 64-bit value for the next generation, and increases the current generation by 1.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long NextRaw()
        {
            return GetRaw(Seed, Generation++);
        }

        /// <summary>
        /// Calculates the raw 64-bit value for the previous generation, and decreases the current generation by 1.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long PrevRaw()
        {
            return GetRaw(Seed, --Generation);
        }

        /// <summary>
        /// Sets the current generation to zero.
        /// </summary>
        public void Reset()
        {
            Generation = 0;
        }

        /// <summary>
        /// Sets the seed to the specified value and the current generation to zero.
        /// </summary>
        /// <param name="newSeed">The new seed to apply to the generator.</param>
        public void Reset(long newSeed)
        {
            Generation = 0;
            Seed = newSeed;
        }

        /// <summary>
        /// Creates a new branch at the specified generation.
        /// </summary>
        /// <param name="generation">The generation to branch from.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RNG Branch(long generation)
        {
            _sg.Add(new SG(GetRaw(Seed, generation), 0));
            return this;
        }

        /// <summary>
        /// The current branching depth of the generator.
        /// </summary>
        public int Depth
        {
            get { return _sg.Count; }
        }

        /// <summary>
        /// Removes the topmost branch and resumes generation on the next one down.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Merge()
        {
            if (_sg.Count > 1)
            {
                _sg.RemoveAt(_sg.Count - 1);
            }
            else
            {
                throw new InvalidOperationException("Cannot merge the base branch.");
            }
        }

        /// <summary>
        /// Calculates a 32-bit, non-negative integer for the current generation.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Peek()
        {
            return (int)GetRaw(Seed, Generation) & 0x7FFFFFFF;
        }

        /// <summary>
        /// Calculates the 32-bitnon-negative integer for the specified generation.
        /// </summary>
        /// <param name="generation">The generation to peek at.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekAt(long generation)
        {
            return (int)GetRaw(Seed, generation) & 0x7FFFFFFF;
        }

        /// <summary>
        /// Returns a double-precision floating point number between 0 and 1, and advances the generation by 1.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NextDouble()
        {
            return Math.Abs(BitConverter.Int64BitsToDouble(NextRaw()) / Double.MaxValue);
        }

        /// <summary>
        /// Calculates a 32-bit, non-negative integer from the next generation and increases the current generation by 1.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next()
        {
            return (int)NextRaw() & 0x7FFFFFFF;
        }

        /// <summary>
        /// Calculates a 32-bit, non-negative integer from the previous generation and decreases the current generation by 1.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Prev()
        {
            return (int)PrevRaw() & 0x7FFFFFFF;
        }

        /// <summary>
        /// Calculates a 32-bit integer between 0 and a specified upper bound for the current generation and increases the current generation by 1.
        /// </summary>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next(int max)
        {
            return (int)(NextRaw() & 0x7FFFFFFF) % max;
        }

        /// <summary>
        /// Calculates a 32-bit integer between 0 and a specified upper bound from the previous generation and decreases the current generation by 1.
        /// </summary>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Prev(int max)
        {
            return (int)(PrevRaw() & 0x7FFFFFFF) % max;
        }

        /// <summary>
        /// Calculates a 32-bit integer between 0 and a specified upper bound for the current generation.
        /// </summary>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Peek(int max)
        {
            return ((int)GetRaw(Seed, Generation) & 0x7FFFFFFF) % max;
        }

        /// <summary>
        /// Calculates a 32-bit integer between 0 and a specified upper bound for the specified generation.
        /// </summary>
        /// <param name="generation">The generation whose value to calculate.</param>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekAt(long generation, int max)
        {
            return ((int)GetRaw(Seed, generation) & 0x7FFFFFFF) % max;
        }

        /// <summary>
        /// Calculates a 32-bit integer between the specified minimum and maximum values for the current generation, and increases the current generation by 1.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next(int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentException("Min must be less than max.");
            }

            return (((int)NextRaw() & 0x7FFFFFFF) - min) % (max - min) + min;
        }

        /// <summary>
        /// Calculates a 32-bit integer between the specified minimum and maximum values for the previous generation, and decreases the current generation by 1.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Prev(int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentException("Min must be less than max.");
            }

            return (((int)PrevRaw() & 0x7FFFFFFF) - min) % (max - min) + min;
        }

        /// <summary>
        /// Calculates a 32-bit integer between the specified minimum and maximum values for the current generation.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Peek(int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentException("Min must be less than max.");
            }

            return (((int)GetRaw(Seed, Generation) & 0x7FFFFFFF) - min) % (max - min) + min;
        }

        /// <summary>
        /// Calculates a 32-bit integer between the specified minimum and maximum values for the specified generation.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The exclusive maximum value.</param>
        /// <param name="generation">The generation whose value to calculate.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekAt(int generation, int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentException("Min must be less than max.");
            }

            return (((int)GetRaw(Seed, generation) & 0x7FFFFFFF) - min) % (max - min) + min;
        }
    }
}