using System.Collections.Generic;
using System.Linq;

namespace Rant.Formatting
{
    /// <summary>
    /// Describes language-specific formatting instructions for localizing interpreter output.
    /// </summary>
    public sealed class RantFormat
    {
        /// <summary>
        /// US English formatting.
        /// </summary>
        public static readonly RantFormat English = new RantFormat(
            IndefiniteArticle.English,
            new[] { "a", "an", "the", "that", "where", "when", "for", "any", "or", "and", "of", "in", "at", "as", "into", "if", "are", "you", "why", "from" });

        private readonly HashSet<string> _titleCaseExclusions;

        /// <summary>
        /// The indefinite article to use in the format.
        /// </summary>
        public IndefiniteArticle IndefiniteArticle { get; private set; } = IndefiniteArticle.English;

        /// <summary>
        /// The words to exclude from Title Case capitalization.
        /// </summary>
        public IEnumerable<string> TitleCaseExclusions => _titleCaseExclusions.AsEnumerable();

        /// <summary>
        /// Creates a new RantFormatStyle instance with the specified arguments.
        /// </summary>
        /// <param name="indefiniteArticle">The indefinite article to use in the format.</param>
        /// <param name="titleCaseExclusions">The words to exclude from Title Case capitalization.</param>
        public RantFormat(IndefiniteArticle indefiniteArticle, string[] titleCaseExclusions)
        {
            IndefiniteArticle = indefiniteArticle;
            _titleCaseExclusions = new HashSet<string>(titleCaseExclusions);
        }

        internal bool Excludes(string word) => _titleCaseExclusions.Contains(word);
    }
}
