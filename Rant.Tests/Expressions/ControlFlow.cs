using NUnit.Framework;

namespace Rant.Tests.Expressions
{
    [TestFixture]
    public class ControlFlow
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void IfStatement()
        {
            Assert.AreEqual("2", rant.Do("[@ if(yes) 2 ]").Main);
        }

        [Test]
        public void WhileLoop()
        {
            Assert.AreEqual("4", rant.Do("[@ x = 10; while(x > 4) { x -= 1; } x]").Main);
        }

        [Test]
        public void WhileLoopBreak()
        {
            Assert.AreEqual("6", rant.Do("[@ x = 10; while(x > 2) { if(x <= 6) break; x -= 1; } x]").Main);
        }

        [Test]
        public void ForLoop()
        {
            Assert.AreEqual("01234", rant.Do("[@ x = list 5; for(var i in x) Console.print(i); ]").Main);
        }
    }
}
