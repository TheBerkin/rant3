namespace Rant.Engine
{
    /// <summary>
    /// Provides visibility settings for output channels.
    /// </summary>
    internal enum ChannelVisibility
    {
        /// <summary>
        /// Channel outputs to itself and 'main'.
        /// </summary>
        Public,
        /// <summary>
        /// Channel outputs only to itself.
        /// </summary>
        Private,
        /// <summary>
        /// Channel outputs only to itself and any parent channels also set to Internal.
        /// </summary>
        Internal
    }
}