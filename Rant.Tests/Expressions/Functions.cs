using NUnit.Framework;

namespace Rant.Tests.Expressions
{
    [TestFixture]
    public class Functions
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void BasicFunction()
        {
            Assert.AreEqual("2", rant.Do("[@ x = function() { 2 }; x() ]").Main);
        }

        [Test]
        public void BasicFunctionArgs()
        {
            Assert.AreEqual("2", rant.Do("[@ x = function(a) { a }; x(2) ]").Main);
        }

        [Test]
        public void LambdaFunction()
        {
            Assert.AreEqual("2", rant.Do("[@ x = () => 2; x() ]").Main);
        }

        [Test]
        public void LambdaFunctionArgs()
        {
            Assert.AreEqual("2", rant.Do("[@ x = (a) => a; x(2) ]").Main);
        }

        [Test]
        public void LambdaEndExpression()
        {
            Assert.AreEqual("2", rant.Do("[@ x = () => 2 ][@ x() ]").Main);
        }

        [Test]
        public void ReturnValue()
        {
            Assert.AreEqual("4", rant.Do("[@ x = function() { return 4; 2 }; x() ]").Main);
        }

        [Test]
        public void MultipleArgs()
        {
            Assert.AreEqual("8", rant.Do("[@ x = function(a, b) { a - b; }; x(10, 2) ]").Main);
        }
    }
}
