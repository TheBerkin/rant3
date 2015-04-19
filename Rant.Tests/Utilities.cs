using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class Utilities
	{
		[Test]
		public void FunctionExists()
		{
			Assert.IsTrue(RantUtils.FunctionExists("rep")); // Inherited function name
			Assert.IsTrue(RantUtils.FunctionExists("x"));	// Alias
		}

		[Test]
		public void FunctionDescription()
		{
			Assert.AreEqual("Sets the repetition count for the next block.", RantUtils.GetFunctionDescription("rep"));
		}
	}
}