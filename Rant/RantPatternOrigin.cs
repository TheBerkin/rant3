namespace Rant
{
    /// <summary>
    /// Indicates the manner in which a referenced code source was created.
    /// </summary>
    public enum RantPatternOrigin
    {
        /// <summary>
        /// Source was loaded from a file.
        /// </summary>
        File,
        /// <summary>
        /// Source was loaded from a string.
        /// </summary>
        String
    }
}