using NUnit.Framework;

namespace Rant.Tests
{
    [TestFixture]
    public class Targets
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void SingleTargetOnce()
        {
            Assert.AreEqual(rant.Do(@"The quick brown [t:a] jumps over the lazy dog.[send:a;fox]").MainValue,
                "The quick brown fox jumps over the lazy dog.");
        }

        [Test]
        public void MultipleTargets()
        {
            Assert.AreEqual(rant.Do(@"The quick brown [t:a] jumps over the lazy [t:b].[send:a;fox][send:b;dog]").MainValue,
                "The quick brown fox jumps over the lazy dog.");
        }

        [Test]
        public void SingleTargetMulti()
        {
            Assert.AreEqual(rant.Do(@"[t:a]-[t:a]-[t:a]-[t:a][send:a;ABCDE]").MainValue,
                "ABCDE-ABCDE-ABCDE-ABCDE");
        }
    }
}