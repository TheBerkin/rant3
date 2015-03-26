namespace Rant.Engine
{
    /// <summary>
    /// Defines parsing modes for the VM.
    /// </summary>
    internal enum ParseMode
    {
        /// <summary>
        /// Parses tags, queries, blocks, and plain text.
        /// </summary>
        Pattern,
        /// <summary>
        /// Parses object model expressions, such as arithmetic and list operations.
        /// </summary>
        Expression
    }
}