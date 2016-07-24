namespace Rant.Internals.Engine.Framework
{
    /// <summary>
    /// Defines parameter types for Rant functions.
    /// </summary>
	public enum RantFunctionParameterType
    {
        /// <summary>
        /// Parameter is a static string.
        /// </summary>
        String,
        /// <summary>
        /// Parameter is a lazily evaluated pattern.
        /// </summary>
        Pattern,
        /// <summary>
        /// Parameter is numeric.
        /// </summary>
        Number,
        /// <summary>
        /// Parameter describes a mode, which is one of a specific set of allowed values.
        /// </summary>
        Mode,
        /// <summary>
        /// Parameter uses combinable flags.
        /// </summary>
        Flags,
        /// <summary>
        /// Parameter is a RantObject.
        /// </summary>
        RantObject
    }
}