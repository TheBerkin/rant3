using NUnit.Framework;

namespace Rant.Tests.Richard
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

        [Test]
        public void LambdaInGroup()
        {
            Assert.AreEqual("By golly, it worked!", rant.Do("[@ (() => { return \"By golly, it worked!\"; })(); ]").Main);
        }

        [Test]
        public void FuncTestIsDefined()
        {
            Assert.AreEqual("true", rant.Do(@"[@{var x = 0; var isDefined = (v) => { v != ???; }; isDefined(x)}]").Main);
        }

        [Test]
        public void FuncTestMinimum()
        {
            var output = rant.Do(@"[x:_;ordered][repeach][sep:,\s]
[@{
var min = function(a, b) {
    if (a < b) return a;
    return b;
};
return [min(15, 8), min(4, 10)];
}]");
            Assert.AreEqual("8, 4", output.Main);
        }
    }
}
