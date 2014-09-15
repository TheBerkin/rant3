namespace Rant.Compiler
{
    /// <summary>
    /// Indicates the manner in which a referenced code source was created.
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        /// Source was loaded from a file.
        /// </summary>
        File,
        /// <summary>
        /// Source was loaded from a string.
        /// </summary>
        String,
        /// <summary>
        /// Source was generated from a metapattern.
        /// </summary>
        Metapattern,
        /// <summary>
        /// Source was generated from an interpreted subroutine definition.
        /// </summary>
        SelfGenerated
    }
}