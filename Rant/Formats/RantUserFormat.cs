namespace Rant.Formats
{
    /// <summary>
    /// Represents a user-specified Rant format.
    /// </summary>
    public sealed class RantUserFormat : RantFormat
    {
        #region Quotation marks

        /// <summary>
        /// The opening primary quotation mark.
        /// </summary>
        public new char OpeningPrimaryQuote { get; set; } = '\u201c';
        /// <summary>
        /// The closing primary quotation mark.
        /// </summary>
        public new char ClosingPrimaryQuote { get; set; } = '\u201d';

        /// <summary>
        /// The opening secondary quotation mark.
        /// </summary>
        public new char OpeningSecondaryQuote { get; set; } = '\u2018';
        /// <summary>
        /// The closing secondary quotation mark.
        /// </summary>
        public new char ClosingSecondaryQuote { get; set; } = '\u2019';

        #endregion

        /// <summary>
        /// The vowel-sensitive indefinite articles used by the \a escape sequence.
        /// </summary>
        public new IndefiniteArticles IndefiniteArticles { get; set; } = IndefiniteArticles.English;
    }
}