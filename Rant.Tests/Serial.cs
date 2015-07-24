using System.Linq;

using NUnit.Framework;

namespace Rant.Tests
{
    [TestFixture]
    public class Serial
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void Single()
        {
            Assert.IsTrue(rant.DoSerial(@"Hello world[yield]").Select(o => o.Main).SequenceEqual(new[] { "Hello world" }));
        }

        [Test]
        public void Multiple()
        {
            Assert.IsTrue(rant.DoSerial(@"[rs:3;\s]{foo}[yield][rs:3;\s]{bar}[yield]").Select(o => o.Main).SequenceEqual(new[] { "foo foo foo", "bar bar bar" }));
        }
    }
}