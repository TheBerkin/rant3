using NUnit.Framework;

namespace Rant.Tests.Expressions.StdLib
{
    [TestFixture]
    public class Type
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void TypeGet()
        {
            Assert.AreEqual("string", rant.Do("[@ Type.get(\"test\") ]").Main);
        }
    }
}
