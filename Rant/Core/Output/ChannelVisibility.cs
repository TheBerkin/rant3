using Rant.Metadata;

namespace Rant.Core.Output
{
    /// <summary>
    /// Provides visibility settings for output channels.
    /// </summary>
    public enum ChannelVisibility
    {
        /// <summary>
        /// Channel outputs to itself and 'main'.
        /// </summary>
        [RantDescription("Channel outputs to itself and 'main'.")]
        Public,
        /// <summary>
        /// Channel outputs only to itself.
        /// </summary>
        [RantDescription("Channel outputs only to itself.")]
        Private,
        /// <summary>
        /// Channel outputs only to itself and any parent channels also set to Internal.
        /// </summary>
        [RantDescription("Channel outputs only to itself and all immediate parent channels also set to Internal.")]
        Internal
    }
}