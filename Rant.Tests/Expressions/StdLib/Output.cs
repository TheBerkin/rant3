using NUnit.Framework;

namespace Rant.Tests.Expressions.StdLib
{
    [TestFixture]
    public class Output
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void OutputPrint()
        {
            Assert.AreEqual("test string", rant.Do("[@ Output.print(\"test string\") ]").Main);
        }
    }
}
