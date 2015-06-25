using NUnit.Framework;

namespace Rant.Tests.Expressions
{
	[TestFixture]
	public class Blocks
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		public void MultipleLines()
		{
            Assert.AreEqual("2", rant.Do("[@ { 4; 2 } ]").Main);
		}
	}
}
