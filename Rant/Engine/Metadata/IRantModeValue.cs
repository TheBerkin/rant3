namespace Rant.Engine.Metadata
{
    /// <summary>
    /// Provides information on Rant's mode values, like number formats and synchronizer types.
    /// </summary>
    public interface IRantModeValue
    {
        /// <summary>
        /// Gets the name of the value.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the description for the value.
        /// </summary>
        string Description { get; }
    }
}