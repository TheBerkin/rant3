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
			Assert.AreEqual("This Is a Test of the Automatic Capitalization", 
				rant.Do(@"[`.+`:this is a test of the automatic capitalization;[caps:title][match]]").Main);
        }

        [TestCase(@"[caps:sentence]this is a sentence. this is another sentence.", "This is a sentence. This is another sentence.")]
        [TestCase(@"[caps:sentence][numfmt:verbal][n:1] is a number.", "One is a number.")]
        public void SentenceCase(string pattern, string expected)
        {
            Assert.AreEqual(expected, rant.Do(pattern).Main);
        }

		[Test]
		public void QuotedSentence()
		{
			Assert.AreEqual("“This is a sentence. This is another sentence.”", 
				rant.Do(@"[quot:[caps:sentence]this is a sentence. this is another sentence.]").Main);
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