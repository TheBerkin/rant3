using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class Blocks
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		public void BasicRepeater()
		{
			Assert.AreEqual("blahblahblahblahblah",
				rant.Do(@"[r:5]{blah}").MainValue);
		}

		[Test]
		public void NestedRepeaters()
		{
			Assert.AreEqual("ABBBABBBABBB",
				rant.Do(@"[r:3]{A[r:3]{B}}").MainValue);
		}

		[Test]
		public void RepeaterSeparator()
		{
			Assert.AreEqual("1, 2, 3, 4, 5",
				rant.Do(@"[r:5][s:,\s]{[rn]}").MainValue);
		}

		[Test]
		public void RepeaterSeparatorNested()
		{
			Assert.AreEqual("(1), (1 2), (1 2 3), (1 2 3 4), (1 2 3 4 5)",
				rant.Do(@"[rs:5;,\s][before:(][after:)]{[rs:[rn];\s]{[rn]}}").MainValue);
		}

		[Test]
		public void BlockDepth()
		{
			Assert.AreEqual("0, 1, 2, 3",
				rant.Do(@"[depth], {[depth]}, {{[depth]}}, {{{[depth]}}}").MainValue);
		}
	}
}