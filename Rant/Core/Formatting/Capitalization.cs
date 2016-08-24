using Rant.Metadata;

namespace Rant.Core.Formatting
{
	internal enum Capitalization
	{
		[RantDescription("No capitalization.")] None,
		[RantDescription("Convert to lowercase.")] Lower,
		[RantDescription("Convert to uppercase.")] Upper,
		[RantDescription("Convert to title case.")] Title,
		[RantDescription("Capitalize the first letter.")] First,
		[RantDescription("Capitalize the first letter of every sentence.")] Sentence,
		[RantDescription("Capitalize the first letter of every word.")] Word
	}
}