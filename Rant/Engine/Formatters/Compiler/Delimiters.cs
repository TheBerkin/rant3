using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Engine.Compiler
{
    internal class Delimiters : IEnumerable<Tuple<R, R>>
    {
        #region Static members

        /// <summary>
        /// Used everywhere except for reading constant literals.
        /// </summary>
        public static readonly Delimiters All = new Delimiters
        {
            {R.Backtick, R.Backtick},
            {R.LeftAngle, R.RightAngle},
            {R.LeftSquare, R.RightSquare},            
            {R.LeftCurly, R.RightCurly}
        };

        #endregion

        private readonly List<Tuple<R, R>> _pairs;
        private readonly HashSet<R> _openings;
        private readonly HashSet<R> _closings; 

        public Delimiters()
        {
            _pairs = new List<Tuple<R, R>>(5);
            _openings = new HashSet<R>();
            _closings = new HashSet<R>();
        }

        public void Add(R openingToken, R closingToken)
        {
            if (_openings.Contains(closingToken) || _closings.Contains(openingToken))
                throw new InvalidOperationException("One or both of the specified tokens already exist as a pair with the reverse order.");
            if (!_openings.Add(openingToken))
                throw new InvalidOperationException("The specified opening token is already defined in another pair in this set.");
            if (!_closings.Add(closingToken))
                throw new InvalidOperationException("The specified closing token is already defined in another pair in this set.");
            
            _pairs.Add(Tuple.Create(openingToken, closingToken));
        }

        public bool Contains(R opening, R closing)
        {
            return _pairs.Any(pair => pair.Item1 == opening && pair.Item2 == closing);
        }

        public bool ContainsOpening(R openingToken)
        {
            return _openings.Contains(openingToken);
        }

        public bool ContainsClosing(R closingToken)
        {
            return _closings.Contains(closingToken);
        }

        public R GetOpening(R closingToken)
        {
            foreach (var pair in _pairs)
            {
                if (pair.Item2 == closingToken) return pair.Item1;
            }
            throw new KeyNotFoundException("Cannot find the specified closing token type in this set.");
        }

        public R GetClosing(R openingToken)
        {
            foreach (var pair in _pairs)
            {
                if (pair.Item1 == openingToken) return pair.Item2;
            }
            throw new KeyNotFoundException("Cannot find the specified opening token type in this set.");
        }

        public IEnumerator<Tuple<R, R>> GetEnumerator()
        {
            return _pairs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}