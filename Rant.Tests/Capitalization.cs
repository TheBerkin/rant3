using NUnit.Framework;

namespace Rant.Tests
{
    [TestFixture]
    public class Capitalization
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void Uppercase()
        {
            Assert.AreEqual(rant.Do(@"[caps:upper]hello world").MainValue, "HELLO WORLD");
        }

        [Test]
        public void Lowercase()
        {
            Assert.AreEqual(rant.Do(@"[caps:lower]HeLlO wOrLd").MainValue, "hello world");
        }

        [Test]
        public void TitleCase()
        {
            Assert.AreEqual(rant.Do(@"[caps:title]this is a title").MainValue, "This Is a Title");
        }

        [Test]
        public void SentenceCase()
        {
            Assert.AreEqual(rant.Do(@"[caps:sentence]this is a sentence. this is another sentence.").MainValue,
                "This is a sentence. This is another sentence.");
        }

        [Test]
        public void FirstCase()
        {
            Assert.AreEqual(rant.Do(@"[caps:first]hello world").MainValue, "Hello world");
        }

        [Test]
        public void WordCase()
        {
            Assert.AreEqual(rant.Do(@"[caps:word]hello world").MainValue, "Hello World");
        }
    }
}