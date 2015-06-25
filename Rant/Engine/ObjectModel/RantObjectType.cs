namespace Rant.Engine.ObjectModel
{
    /// <summary>
    /// Defines object types used by Rant.
    /// </summary>
    public enum RantObjectType
    {
        /// <summary>
        /// Represents a decimal number.
        /// </summary>
        Number,
        /// <summary>
        /// Represents a series of Unicode characters.
        /// </summary>
        String,
        /// <summary>
        /// Represents a compiled Rant pattern.
        /// </summary>
        Pattern,
        /// <summary>
        /// Represents a boolean value.
        /// </summary>
        Boolean,
        /// <summary>
        /// Represents a resizable set of values.
        /// </summary>
        List,
		/// <summary>
		/// Represents a VM action.
		/// </summary>
		Action,
        /// <summary>
        /// Represents a lack of a value.
        /// </summary>
        No,
        /// <summary>
        /// Represents a lack of any variable at all.
        /// </summary>
        Undefined
    }
}