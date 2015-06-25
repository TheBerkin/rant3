using NUnit.Framework;

namespace Rant.Tests.Expressions
{
	public class Invalid
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		// this was causing a huge amount of recursion for some reason
		public void SemicolonRecursion()
		{
			rant.Do(@"[@ test = 1; test; ]");
			Assert.Pass();
		}
	}
}
