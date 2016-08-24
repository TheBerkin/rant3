using Rant.Metadata;

namespace Rant.Core.Formatting
{
	internal enum NumberFormat
	{
		[RantDescription("No special formatting.")]
		Normal,

		[RantDescription("Group digits with the system's digit separator.")]
		Group,

		[RantDescription("Group digits by commas.")]
		GroupCommas,

		[RantDescription("Group digits by dots.")]
		GroupDots,

		[RantDescription("Uppercase Roman numerals.")]
		Roman,

		[RantDescription("Uppercase Roman numerals.")]
		RomanUpper,

		[RantDescription("Lowercase Roman numerals.")]
		RomanLower,

		[RantDescription("US English spellings of numbers. Only works with integers.")]
		VerbalEn,

		[RantDescription("Uppercase hexadecimal.")]
		Hex,

		[RantDescription("Uppercase hexadecimal.")]
		HexUpper,

		[RantDescription("Lowercase hexadecimal.")]
		HexLower,

		[RantDescription("Robot language.")]
		Binary
	}
}