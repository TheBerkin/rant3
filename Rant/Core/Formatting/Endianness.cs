using Rant.Metadata;

namespace Rant.Core.Formatting
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