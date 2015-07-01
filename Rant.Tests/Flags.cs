using NUnit.Framework;

namespace Rant.Tests
{
    [TestFixture]
    public class Flags
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void FlagDefine()
        {
            rant.Flags.Clear();
            rant.Do(@"[define:test]");
            Assert.IsTrue(rant.Flags.Contains("test"));
        }

        [Test]
        public void FlagUndefine()
        {
            rant.Flags.Clear();
            rant.Do(@"[define:test][undef:test]");
            Assert.IsTrue(!rant.Flags.Contains("test"));
        }

        [Test]
        public void FlagIfDef()
        {
            rant.Flags.Clear();
            Assert.AreEqual("Test Passed", rant.Do(@"[define:test][ifdef:test][then:Test Passed]").Main);
        }

        [Test]
        public void FlagIfNDef()
        {
            rant.Flags.Clear();
            Assert.AreEqual("Test Passed", rant.Do(@"[ifndef:test][then:Test Passed]").Main);
        }

        [Test]
        public void FlagIfDefLateDefine()
        {
            rant.Flags.Clear();
            Assert.AreEqual("Test Passed", rant.Do(@"[ifdef:test][define:test][then:Test Passed]").Main);
        }

        [Test]
        public void FlagIfNDefLateUndef()
        {
            rant.Flags.Clear();
            Assert.AreEqual("Test Passed", rant.Do(@"[define:test][ifndef:test][undef:test][then:Test Passed]").Main);
        }

        [Test]
        public void FlagIfThenElse()
        {
            rant.Flags.Clear();
            Assert.AreEqual("Test Passed", rant.Do(@"[define:test][ifdef:test][then:Test Passed][else:... NOT!]").Main);
            Assert.AreEqual("Test Passed", rant.Do(@"[define:test][ifndef:test][then:You WISH this\s][else:Test Passed]").Main);
        }

        [Test]
        public void FlagIfDefMulti()
        {
            rant.Flags.Clear();
            Assert.AreEqual("Test Passed", rant.Do(@"[define:a;b;c][ifdef:a;b;c][then:Test Passed][else:... NOT!]").Main);
        }

        [Test]
        public void FlagIfNDefMulti()
        {
            rant.Flags.Clear();
            Assert.AreEqual("Test Passed", rant.Do(@"[ifndef:a;b;c][then:Test Passed][else:... NOT!]").Main);
        }
    }
}