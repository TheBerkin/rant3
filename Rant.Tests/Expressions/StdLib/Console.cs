using NUnit.Framework;

namespace Rant.Tests.Expressions.StdLib
{
    [TestFixture]
    public class Console
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void ConsolePrint()
        {
            Assert.AreEqual("test string", rant.Do("[@ Console.print(\"test string\") ]").Main);
        }
    }
}
