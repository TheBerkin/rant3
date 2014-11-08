namespace Rant.Vocabulary
{
    /// <summary>
    /// Represents a Rant dictionary term. A dictionary entry will contain one term for every subtype.
    /// </summary>
    public sealed class Term
    {
        private string _value;
        private string _pronunciation;

        /// <summary>
        /// Creates a new Term with the specified value.
        /// </summary>
        /// <param name="value">The value of the term.</param>
        public Term(string value)
        {
            Value = value;
            Pronunciation = "";
        }

        /// <summary>
        /// Creates a new Term with the specified value and pronunciation.
        /// </summary>
        /// <param name="value">The value of the term.</param>
        /// <param name="pronunciation">The pronunciation of the term.</param>
        public Term(string value, string pronunciation)
        {
            Value = value;
            Pronunciation = pronunciation;
        }

        /// <summary>
        /// The value of the term.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The pronunciation of the term.
        /// </summary>
        public string Pronunciation
        {
            get { return _pronunciation; }
            set { _pronunciation = value; }
        }
    }
}