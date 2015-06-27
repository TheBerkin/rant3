using NUnit.Framework;

namespace Rant.Tests.Richard.StdLib
{
    [TestFixture]
    public class Strings
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void StringLength()
        {
            Assert.AreEqual("4", rant.Do("[@ \"test\".length ]").Main);
        }
    }
}
