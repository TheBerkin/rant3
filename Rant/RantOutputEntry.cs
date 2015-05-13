namespace Rant
{
    /// <summary>
    /// Represents the output of a single channel.
    /// </summary>
    public sealed class RantOutputEntry
    {
        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the channel.
        /// </summary>
        public string Value { get; }

        internal RantOutputEntry(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}