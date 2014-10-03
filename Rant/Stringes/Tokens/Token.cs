using System;

namespace Stringes.Tokens
{
    /// <summary>
    /// Represents a token that contains a custom identifier.
    /// </summary>
    /// <typeparam name="T">The identifier type.</typeparam>
    public sealed class Token<T> : Stringe where T : struct
    {
        private readonly T _id;

        /// <summary>
        /// The token identifier.
        /// </summary>
        public T Identifier
        {
            get { return _id; }
        }

        public Token(T id, string value) : base(value)
        {
            _id = id;
        }

        public Token(T id, Stringe value) : base(value)
        {
            _id = id;
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Value)) return "<" + _id + ">";
            return String.Concat("<", _id, " @ L", Line, ", C", Column, ": '", Value, "'>");
        }
    }
}