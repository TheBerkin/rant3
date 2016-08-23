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
            Assert.AreEqual("HELLO WORLD", rant.Do(@"[caps:upper]hello world").Main);
        }

        [Test]
        public void Lowercase()
        {
            Assert.AreEqual("hello world", rant.Do(@"[caps:lower]HeLlO wOrLd").Main);
        }

        [Test]
        public void TitleCase()
        {
            Assert.AreEqual("This Is a Title", rant.Do(@"[caps:title]this is a title").Main);
        }

        [Test]
        public void SentenceCase()
        {
            Assert.AreEqual("This is a sentence. This is another sentence.", 
				rant.Do(@"[caps:sentence]this is a sentence. this is another sentence.").Main);
        }

		[Test]
		public void QuotedSentence()
		{
			Assert.AreEqual("“This is a sentence. This is another sentence.”", 
				rant.Do(@"[q:[caps:sentence]this is a sentence. this is another sentence.]").Main);
		}

		[Test]
        public void FirstCase()
        {
            Assert.AreEqual("Hello world", rant.Do(@"[caps:first]hello world").Main);
        }

        [Test]
        public void WordCase()
        {
            Assert.AreEqual("Hello World", rant.Do(@"[caps:word]hello world").Main);
        }
    }
}