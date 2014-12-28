using System;

namespace Rant.Formatting
{
    /// <summary>
    /// Represents a system-specified Rant format that cannot be modified.
    /// </summary>
    public sealed class RantSystemFormat : RantFormat
    {
        internal RantSystemFormat()
        {
        }

        public override void AddTitleCaseExclusions(params string[] words)
        {
            throw new InvalidOperationException("Cannot modify a system format.");
        }
    }
}
