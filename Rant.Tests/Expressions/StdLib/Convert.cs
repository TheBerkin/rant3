using NUnit.Framework;

namespace Rant.Tests.Expressions.StdLib
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
    }
   }
