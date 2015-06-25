using NUnit.Framework;

namespace Rant.Tests.Expressions.StdLib
{
    [TestFixture]
    public class Lists
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void ListLength()
        {
            Assert.AreEqual("4", rant.Do("[@ var x = 1, 2, 3, 4; x.length ]").Main);
        }

        [Test]
        public void ListPush()
        {
            Assert.AreEqual("5", rant.Do("[@ var x = 1, 2, 3, 4; x.push(5); x[4] ]").Main);
        }

        [Test]
        public void ListPop()
        {
            Assert.AreEqual("4", rant.Do("[@ var x = 1, 2, 3, 4, 5; x.pop(); x.length ]").Main);
        }

        [Test]
        public void ListPopFront()
        {
            Assert.AreEqual("1", rant.Do("[@ var x = 1, 2, 3, 4, 5; x.popFront() ]").Main);
        }

        [Test]
        public void ListPushFront()
        {
            Assert.AreEqual("0", rant.Do("[@ var x = 1, 2, 3, 4, 5; x.pushFront(0); x[0] ]").Main);
        }

        [Test]
        public void ListCopy()
        {
            Assert.AreEqual("5", rant.Do("[@ var x = 1, 2, 3, 4, 5; var y = x.copy(); x.pop(); y.length ]").Main);
        }

        [Test]
        public void ListFill()
        {
            Assert.AreEqual("8", rant.Do("[@ var x = list 12; x.fill(8); x[4] ]").Main);
        }

        [Test]
        public void ListReverse()
        {
            Assert.AreEqual("5", rant.Do("[@ var x = 1, 2, 3, 4, 5; x.reverse(); x[0] ]").Main);
        }

        [Test]
        public void ListJoin()
        {
            Assert.AreEqual("1,2,3,4,5", rant.Do("[@ var x = 1, 2, 3, 4, 5; x.join(\",\") ]").Main);
        }

        [Test]
        public void ListInsert()
        {
            Assert.AreEqual("4", rant.Do("[@ var x = 1, 2, 3, 5; x.insert(4, 3); x[3] ]").Main);
        }

        [Test]
        public void ListRemove()
        {
            Assert.AreEqual("4", rant.Do("[@ var x = 1, 2, 3, 4, 5; x.remove(3); x.length ]").Main);
        }
    }
}
