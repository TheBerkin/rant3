using System.Collections.Generic;

using NUnit.Framework;

using Rant.Internals.Engine.ObjectModel;

namespace Rant.Tests
{
    [TestFixture]
    public class GlobalObjects
    {
        [Test]
        public void IntegerGlobal()
        {
            var rant = new RantEngine { ["myInt"] = new RantObject(123) };
            Assert.AreEqual("123", rant.Do(@"[@myInt]").Main);
        }

        [Test]
        public void StringGlobal()
        {
            var rant = new RantEngine {["myString"] = new RantObject("Hello world") };
            Assert.AreEqual("Hello world", rant.Do(@"[@myString]").Main);
        }

        [Test]
        public void ListGlobal()
        {
            var list = new List<RantObject>
            {
                new RantObject("A"),
                new RantObject("B"),
                new RantObject("C"),
                new RantObject("D")
            };
            var rant = new RantEngine {["myList"] = new RantObject(list) };
            Assert.AreEqual("A, B, C, D", rant.Do(@"[@ myList.join("", "") ]").Main);
        }
    }
}