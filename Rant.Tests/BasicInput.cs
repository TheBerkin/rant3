using NUnit.Framework;

namespace Rant.Tests
{
    public class BasicInput
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void RawText()
        {
            Assert.AreEqual("Hello world", rant.Do(@"Hello world").MainValue);
        }

        [Test]
        public void BlockText()
        {
            Assert.AreEqual("Hello world", rant.Do(@"{Hello} world").MainValue);
        }

        [Test]
        public void Blackspace()
        {
            Assert.AreEqual("ABCD", rant.Do("A\n  B\nC  \n  D  ").MainValue);
        }

        [Test]
        public void Comments()
        {
            Assert.AreEqual("Hello world", rant.Do("#This is a comment.\nHello world # This is another comment.").MainValue);
        }

        [Test]
        public void BlockTextWithBlackspace()
        {
            Assert.AreEqual("Hello world", rant.Do(@"{  Hello  } world").MainValue);
        }

        [Test]
        public void EscapeSequences()
        {
            Assert.AreEqual("[Lorem ipsum]", rant.Do(@"\[Lorem ipsum\]").MainValue);
        }

		[Test]
	    public void UnicodeCharacters()
	    {
		    Assert.AreEqual("\u2764", rant.Do(@"\u2764").MainValue);
	    }

		[Test]
		public void QuantifiedUnicodeCharacters()
		{
			Assert.AreEqual(new string('\u2764', 16), rant.Do(@"\16,u2764").MainValue);
        }

		[Test]
        public void QuantifiedEscapeSequence()
        {
            Assert.AreEqual("========================", rant.Do(@"\24,=").MainValue);
        }

        [Test]
        public void Whitespace()
        {
            Assert.AreEqual("        ", rant.Do(@"  { \s \s \4,s  }   ").MainValue);
        }
    }
}
