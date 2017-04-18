using Rant.Metadata;

namespace Rant.Core.Formatting
{
	internal enum CharConversion
	{
		[RantDescription("No conversion.")]
		None,
		[RantDescription("Fullwidth characters.")]
		Fullwidth,
		[RantDescription("Cursive script.")]
		Cursive,
		[RantDescription("Bold cursive script.")]
		BoldCursive
	}
}
