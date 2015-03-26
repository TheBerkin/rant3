using NUnit.Framework;

namespace Rant.Tests.Expressions
{
    [TestFixture]
    public class Variables
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void String()
        {
            Assert.AreEqual("a string", rant.Do("[@ a = 'a string'; a]").MainValue);
        }

        [Test]
        public void Number()
        {
            Assert.AreEqual("200", rant.Do("[@ a = 200; a]").MainValue);
        }

        [Test]
        public void Var()
        {
            // if var does not work, a will be undefined when accessed and will throw
            rant.Do("[@ var a; a ]");
            Assert.Pass();
        }

        [Test]
        [ExpectedException(typeof(RantException))]
        public void LocalScopes()
        {
            // this will throw because a is defined within a local scope
            rant.Do("{[@ a = 1; ]} [@ a ]");
        }

        [Test]
        public void GlobalScopes()
        {
            Assert.AreEqual(rant.Do("[@ var a ] {[@ a = 1]} [@ a ]").MainValue, "1");
        }
    }
}
