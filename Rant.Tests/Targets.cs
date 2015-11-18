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
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.",
				rant.Do(@"The quick brown [t:a] jumps over the lazy dog.[send:a;fox]").Main);
        }

        [Test]
        public void MultipleTargets()
        {
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.",
				rant.Do(@"The quick brown [t:a] jumps over the lazy [t:b].[send:a;fox][send:b;dog]").Main);
        }

        [Test]
        public void SingleTargetMulti()
        {
            Assert.AreEqual("ABCDE-ABCDE-ABCDE-ABCDE", rant.Do(@"[t:a]-[t:a]-[t:a]-[t:a][send:a;ABCDE]").Main);
        }
    }
}