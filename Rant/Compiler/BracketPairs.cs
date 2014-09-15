using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Compiler
{
    internal class BracketPairs : IEnumerable<Tuple<TokenType, TokenType>>
    {
        #region Static members

        /// <summary>
        /// Used everywhere except for reading constant literals.
        /// </summary>
        public static readonly BracketPairs All = new BracketPairs
        {
            {TokenType.LeftAngle, TokenType.RightAngle},
            {TokenType.LeftSquare, TokenType.RightSquare},
            {TokenType.LeftParen, TokenType.RightParen},
            {TokenType.LeftCurly, TokenType.RightCurly}
        };

        #endregion

        private readonly List<Tuple<TokenType, TokenType>> _pairs;
        private readonly HashSet<TokenType> _openings;
        private readonly HashSet<TokenType> _closings; 

        public BracketPairs()
        {
            _pairs = new List<Tuple<TokenType, TokenType>>(5);
            _openings = new HashSet<TokenType>();
            _closings = new HashSet<TokenType>();
        }

        public void Add(TokenType openingToken, TokenType closingToken)
        {
            if (openingToken == closingToken)
                throw new ArgumentException("The opening and closing tokens cannot match. Ever. You monster.");
            if (_openings.Contains(closingToken) || _closings.Contains(openingToken))
                throw new InvalidOperationException("One or both of the specified tokens already exist as a pair with the reverse order.");
            if (!_openings.Add(openingToken))
                throw new InvalidOperationException("The specified opening token is already defined in another pair in this set.");
            if (!_closings.Add(closingToken))
                throw new InvalidOperationException("The specified closing token is already defined in another pair in this set.");
            
            _pairs.Add(Tuple.Create(openingToken, closingToken));
        }

        public bool Contains(TokenType opening, TokenType closing)
        {
            return _pairs.Any(pair => pair.Item1 == opening && pair.Item2 == closing);
        }

        public bool ContainsOpening(TokenType openingToken)
        {
            return _openings.Contains(openingToken);
        }

        public bool ContainsClosing(TokenType closingToken)
        {
            return _closings.Contains(closingToken);
        }

        public TokenType GetOpening(TokenType closingToken)
        {
            foreach (var pair in _pairs)
            {
                if (pair.Item2 == closingToken) return pair.Item1;
            }
            throw new KeyNotFoundException("Cannot find the specified closing token type in this set.");
        }

        public TokenType GetClosing(TokenType openingToken)
        {
            foreach (var pair in _pairs)
            {
                if (pair.Item1 == openingToken) return pair.Item2;
            }
            throw new KeyNotFoundException("Cannot find the specified opening token type in this set.");
        }

        public IEnumerator<Tuple<TokenType, TokenType>> GetEnumerator()
        {
            return _pairs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}