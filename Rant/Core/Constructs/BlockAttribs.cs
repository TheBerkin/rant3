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

using Rant.Core.Compiler.Syntax;

namespace Rant.Core.Constructs
{
    internal class BlockAttribs
    {
        private int _reps = 1;

        public int Repetitions
        {
            get { return _reps; }
            set
            {
                _reps = value;
                RepEach = false;
            }
        }

        public int StartIndex { get; set; } = -1;
        public bool IsSeries { get; set; }
        public bool RepEach { get; set; }
        public RST Separator { get; set; }
        public RST EndSeparator { get; set; }
        public RST EndConjunction { get; set; }
        public RST Start { get; set; }
        public RST Before { get; set; }
        public RST After { get; set; }
        public RST End { get; set; }
        public Synchronizer Sync { get; set; }
        public double Chance { get; set; } = 100.0;
        public AttribPersistence Persistence { get; set; } = AttribPersistence.Off;

        public void SetDefaults()
        {
            Repetitions = 1;
            StartIndex = -1;
            RepEach = false;
            IsSeries = false;
            Chance = 100;
            Separator = null;
            EndSeparator = null;
            EndConjunction = null;
            Before = null;
            After = null;
            Sync = null;
            Start = null;
            End = null;
            Persistence = AttribPersistence.Off;
        }

        /// <summary>
        /// Calculates the index of the next block item to execute.
        /// </summary>
        /// <param name="blockItemCount">The number of items in the block.</param>
        /// <param name="rng">The random number generator to use with index calculation.</param>
        /// <returns></returns>
        public int NextIndex(int blockItemCount, RNG rng)
        {
            // Use synchronizer if available
            if (Sync != null)
                return Sync.NextItem(blockItemCount);

            return rng.Next(blockItemCount);
        }
    }
}