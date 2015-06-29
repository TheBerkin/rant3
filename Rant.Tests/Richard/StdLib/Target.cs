using NUnit.Framework;

namespace Rant.Tests.Richard.StdLib
{
    [TestFixture]
    public class Target
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void TargetGet()
        {
            Assert.AreEqual("testtest", rant.Do("[target: t][send: t; test][@ Target.get(\"t\") ]").Main);
        }

        [Test]
        public void TargetSend()
        {
            Assert.AreEqual("test", rant.Do("[target: t][@ Target.send(\"t\", \"test\") ]").Main);
        }

        [Test]
        public void TargetClear()
        {
            Assert.AreEqual("", rant.Do("[target: t][send: t; test][@ Target.clear(\"t\") ]").Main);
        }
    }
}
