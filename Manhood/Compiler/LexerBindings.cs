using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Manhood.Compiler
{
    /// <summary>
    /// Describes a set of rules for how a lexer should generate tokens.
    /// </summary>
    /// <typeparam name="T">The token identifier type.</typeparam>
    internal class LexerBindings<T>
    {
        private readonly ReadOnlyCollection<Tuple<string, T>> _list;
        private readonly HashSet<char> _punctuation;

        public LexerBindings(params Tuple<string, T>[] items)
        {
            _list = items.OrderByDescending(item => item.Item1).ToList().AsReadOnly();
            _punctuation = new HashSet<char>();
            foreach (var i in _list.Where(item => !String.IsNullOrEmpty(item.Item1)))
            {
                _punctuation.Add(i.Item1[0]);
            }
        }

        /// <summary>
        /// Indicates whether the bindings use the specified punctuation character.
        /// </summary>
        /// <param name="punctuation">The punctuation character to test.</param>
        /// <returns></returns>
        public bool HasPunctuation(char punctuation)
        {
            return _punctuation.Contains(punctuation);
        }

        /// <summary>
        /// The collection of binding definitions.
        /// </summary>
        public IEnumerable<Tuple<string, T>> Items
        {
            get { return _list; }
        }

        public static implicit operator LexerBindings<T>(Tuple<string, T>[] collection)
        {
            return new LexerBindings<T>(collection);
        }
    }
}