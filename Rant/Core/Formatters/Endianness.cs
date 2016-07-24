using Rant.Metadata;

namespace Rant.Core.Formatters
{
    internal enum Endianness
    {
        [RantDescription("Big endian.")]
        Big,
        [RantDescription("Little endian.")]
        Little,
        [RantDescription("Whatever endianness your system uses.")]
        Default
    }
}