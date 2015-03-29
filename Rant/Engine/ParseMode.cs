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
        Rant,
        /// <summary>
        /// Parses object model expressions and statements, such as arithmetic and list operations.
        /// </summary>
        Rave
    }
}