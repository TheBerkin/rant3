using NUnit.Framework;

namespace Rant.Tests
{
    [TestFixture]
    public class Repeaters
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void BasicRepeater()
        {
            Assert.AreEqual(rant.Do(@"[rep:5]{blah}").MainValue, "blahblahblahblahblah");
        }

        [Test]
        public void NestedRepeaters()
        {
            Assert.AreEqual(rant.Do(@"[rep:3]{A[rep:3]{B}}").MainValue, "ABBBABBBABBB");
        }

        [Test]
        public void RepeaterSeparator()
        {
            Assert.AreEqual(rant.Do(@"[rep:5][sep:,\s]{[repnum]}").MainValue, "1, 2, 3, 4, 5");
        }

        [Test]
        public void RepeaterSeparatorNested()
        {
            Assert.AreEqual(rant.Do(@"[rep:5][sep:,\s][before:(][after:)]{[rep:[repnum]][sep:\s]{[repnum]}}").MainValue,
                "(1), (1 2), (1 2 3), (1 2 3 4), (1 2 3 4 5)");
        }
    }
}