using NUnit.Framework;

namespace Rant.Tests.Richard.StdLib
{
    [TestFixture]
    public class Convert
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void ConvertToString()
        {
            Assert.AreEqual("5", rant.Do("[@ Convert.toString(5); ]").Main);
        }

        [Test]
        public void ConvertToNumber()
        {
            Assert.AreEqual("4.2", rant.Do("[@ Convert.toNumber(\"4.2\") ]").Main);
        }
    }
   }
