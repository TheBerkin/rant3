using NUnit.Framework;

using Rant.Core.Stringes;

namespace Rant.Tests.Misc
{
	[TestFixture]
	public class Stringes
	{
		[Test]
		public void GlobalCol()
		{
			var s = new Stringe("Example");
			Assert.AreEqual(1, s.Column);
		}

		[Test]
		public void GlobalLine()
		{
			var s = new Stringe("Example");
			Assert.AreEqual(1, s.Line);
		}
	}
}