using NUnit.Framework;
using System.Linq;

namespace Rant.Tests.Expressions
{
    [TestFixture]
    public class Lists
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void ListInitBare()
        {
            Assert.AreEqual("5", rant.Do("[@ x = 1, 2, 3, 4, 5; x.length ]").Main);
        }

        [Test]
        public void ListInitBrackets()
        {
            Assert.AreEqual("5", rant.Do("[@ x = [1, 2, 3, 4, 5]; x.length ]").Main);
        }

        [Test]
        public void ListInitConcat()
        {
            Assert.AreEqual("5", rant.Do("[@ x = 1, 2, 3; y = x, 4, 5; y.length ]").Main);
        }

        [Test]
        public void ListAsBlock()
        {
            string[] possibleResults = new string[] { "1", "2", "3", "4" };
            Assert.GreaterOrEqual(possibleResults.ToList().IndexOf(rant.Do("[@ x = 1, 2, 3, 4; x ]").Main), 0);
        }
    }
}
