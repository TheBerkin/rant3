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
				rant.Do(@"[r:5]{blah}").Main);
		}

		[Test]
		public void NestedRepeaters()
		{
			Assert.AreEqual("ABBBABBBABBB",
				rant.Do(@"[r:3]{A[r:3]{B}}").Main);
		}

		[Test]
		public void RepeaterSeparator()
		{
			Assert.AreEqual("1, 2, 3, 4, 5",
				rant.Do(@"[r:5][s:,\s]{[rn]}").Main);
		}

		[Test]
		public void RepeaterSeparatorNested()
		{
			Assert.AreEqual("(1), (1 2), (1 2 3), (1 2 3 4), (1 2 3 4 5)",
				rant.Do(@"[rs:5;,\s][before:(][after:)]{[rs:[rn];\s]{[rn]}}").Main);
		}

		[Test]
		public void BlockDepth()
		{
			Assert.AreEqual("0, 1, 2, 3",
				rant.Do(@"[depth], {[depth]}, {{[depth]}}, {{{[depth]}}}").Main);
		}

        [Test]
        public void PersistenceOn()
        {
            Assert.AreEqual("aaabbb",
                rant.Do(@"[persist:on][r:3]{a}{b}").Main);
        }

        [Test]
        public void PersistenceOff()
        {
            Assert.AreEqual("aaab",
                rant.Do(@"[persist:off][r:3]{a}{b}").Main);
        }

        [Test]
        public void PersistenceOnce()
        {
            Assert.AreEqual("aaabbbc",
                rant.Do(@"[persist:once][r:3]{a}{b}{c}").Main);
        }

        [Test]
        public void PersistenceOuter()
        {
            Assert.AreEqual("ababab",
                rant.Do(@"[persist:outer][r:3]{{a}{b}}").Main);
        }
    }
}