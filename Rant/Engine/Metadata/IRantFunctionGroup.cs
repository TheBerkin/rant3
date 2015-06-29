using System.Collections.Generic;

namespace Rant.Engine.Metadata
{
    /// <summary>
    /// Provides access to metadata for a group of overloads for a specific Rant function.
    /// </summary>
    public interface IRantFunctionGroup
    {
        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the available overloads for the function.
        /// </summary>
        IEnumerable<IRantFunction> Overloads { get; }
    }
}