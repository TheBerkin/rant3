using NUnit.Framework;

namespace Rant.Tests
{
    [TestFixture]
    public class Numbers
    {
        private readonly RantEngine rant = new RantEngine();

		[Test]
	    public void RandomNumbers()
	    {
		    var output = rant.Do(@"[r:10][s:\s]{[n:1;100]}").MainValue.Split(' ');
			Assert.AreEqual(10, output.Length);
		    int i;
		    foreach (var numberString in output)
		    {
			    Assert.True(int.TryParse(numberString, out i));
				Assert.True(i >= 1 && i <= 100);
		    }
	    }

        [Test]
		public void Verbal()
        {
            Assert.AreEqual(rant.Do(@"[r:10][s:,\s][numfmt:verbal-en]{[rn]}").MainValue,
                "one, two, three, four, five, six, seven, eight, nine, ten");
        }

        [Test]
        public void Hex8()
        {
            Assert.AreEqual(rant.Do(@"[numfmt:hex][digits:pad;2][n:8] [n:15] [n:55] [n:128] [n:255]").MainValue,
                "08 0F 37 80 FF");
        }

        [Test]
        public void Hex16()
        {
            Assert.AreEqual(rant.Do(@"[numfmt:hex][digits:pad;4][n:12] [n:64] [n:456] [n:5000] [n:65535]").MainValue,
                "000C 0040 01C8 1388 FFFF");
        }

        [Test]
        public void Hex16BigEndian()
        {
            Assert.AreEqual(rant.Do(@"[numfmt:hex][endian:big][digits:pad;4][n:12] [n:64] [n:456] [n:5000] [n:65535]").MainValue,
                "0C00 4000 C801 8813 FFFF");
        }

        [Test]
        public void DigitGrouping()
        {
            Assert.AreEqual(rant.Do(@"[numfmt:group-commas][n:1000] [n:10000] [n:32768] [n:300000] [n:1000000]").MainValue,
                "1,000 10,000 32,768 300,000 1,000,000");
        }

        [Test]
        public void RomanNumerals()
        {
            Assert.AreEqual(rant.Do(@"[numfmt:roman][rep:12][s:,\s]{[rn]}").MainValue,
                "I, II, III, IV, V, VI, VII, VIII, IX, X, XI, XII");
        }

        [Test]
        public void Binary()
        {
            Assert.AreEqual(rant.Do(@"[numfmt:binary][digits:pad;1][rep:16][sep:\s]{[re]}").MainValue,
                "0000 0001 0010 0011 0100 0101 0110 0111 1000 1001 1010 1011 1100 1101 1110 1111");
        }
    }
}