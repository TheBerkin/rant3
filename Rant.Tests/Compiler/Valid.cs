using NUnit.Framework;

namespace Rant.Tests.Compiler
{
	[TestFixture]
	public class Valid
	{
		[Test]
		public void Plaintext()
		{
			RantPattern.FromString(@"just some text");
		}

		[TestCase(@"{}")]
		[TestCase(@"{|}")]
		[TestCase(@"{||}")]
		[TestCase(@"{Item 1}")]
		[TestCase(@"{Item 1|Item 2}")]
		[TestCase(@"{Item 1|Item 2|Item 3}")]
		public void Block(string pattern)
		{
			RantPattern.FromString(pattern);
		}
	}
}