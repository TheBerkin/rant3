using Rant.Engine.Metadata;

namespace Rant.Engine.Formatters
{
    internal enum BinaryFormat
    {
        [RantDescription("Use as many digits as necessary to accomodate the number.")]
        Normal,
        [RantDescription("Pad numbers to a specific number of digits.")]
        Pad,
        [RantDescription("Truncate numbers over a specific number of digits.")]
        Truncate
    }
}