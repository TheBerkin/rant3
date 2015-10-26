using NUnit.Framework;

namespace Rant.Tests
{
    public class BasicInput
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void RawText()
        {
            Assert.AreEqual("Hello world", rant.Do(@"Hello world").Main);
        }

        [Test]
        public void BlockText()
        {
            Assert.AreEqual("Hello world", rant.Do(@"{Hello} world").Main);
        }

        [Test]
        public void Blackspace()
        {
            Assert.AreEqual("ABCD", rant.Do("A\n  B\nC  \n  D  ").Main);
        }

        [Test]
        public void Comments()
        {
            Assert.AreEqual("Hello world", rant.Do("#This is a comment.\nHello world # This is another comment.").Main);
        }

        [Test]
        public void BlockTextWithBlackspace()
        {
            Assert.AreEqual("Hello world", rant.Do(@"{  Hello  } world").Main);
        }

        [Test]
        public void EscapeSequences()
        {
            Assert.AreEqual("[Lorem ipsum]", rant.Do(@"\[Lorem ipsum\]").Main);
        }

		[Test]
	    public void UnicodeCharacters()
	    {
		    Assert.AreEqual("\u2764", rant.Do(@"\u2764").Main);
	    }

		[Test]
		public void QuantifiedUnicodeCharacters()
		{
			Assert.AreEqual(new string('\u2764', 16), rant.Do(@"\16,u2764").Main);
        }

		[Test]
        public void QuantifiedEscapeSequence()
        {
            Assert.AreEqual("========================", rant.Do(@"\24,=").Main);
        }

        [Test]
        public void Whitespace()
        {
            Assert.AreEqual("        ", rant.Do(@"  { \s \s \4,s  }   ").Main);
        }

        [Test]
        public void SymbolFunctions()
        {
            Assert.AreEqual(
                "\x2122 \x00ae \x00a9 \x2014 \x2013 \x2022 \x00df",
                rant.Do(@"[tm] [reg] [c] [em] [en] [b] [ss]").Main
            );
        }

        [Test]
        public void Emoji()
        {
            Assert.AreEqual("🔫😠", rant.Do(@"[emoji:gun][emoji:angry]").Main);
        }
    }
}
