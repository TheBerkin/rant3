using NUnit.Framework;

namespace Rant.Tests
{
    public class BasicInput
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void RawText()
        {
            Assert.AreEqual(rant.Do(@"Hello world").MainValue, "Hello world");
        }

        [Test]
        public void BlockText()
        {
            Assert.AreEqual(rant.Do(@"{Hello} world").MainValue, "Hello world");
        }

        [Test]
        public void Blackspace()
        {
            Assert.AreEqual(rant.Do("A\n  B\nC  \n  D  ").MainValue, "ABCD");
        }

        [Test]
        public void Comments()
        {
            Assert.AreEqual(rant.Do("#This is a comment.\nHello world # This is another comment.").MainValue, "Hello world");
        }

        [Test]
        public void BlockTextWithBlackspace()
        {
            Assert.AreEqual(rant.Do(@"{  Hello  } world").MainValue, "Hello world");
        }

        [Test]
        public void EscapeSequences()
        {
            Assert.AreEqual(rant.Do(@"\[Lorem ipsum\]").MainValue, "[Lorem ipsum]");
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
            Assert.AreEqual(rant.Do(@"\24,=").MainValue, "========================");
        }

        [Test]
        public void Whitespace()
        {
            Assert.AreEqual(rant.Do(@"  { \s \s \4,s  }   ").MainValue, "        ");
        }
    }
}
