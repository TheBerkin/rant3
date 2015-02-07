using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rant.Tests
{
    [TestFixture]
    public class Replacers
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void ReplacerOverflow()
        {
            rant.Do("[//x//:[char:x;64];y]");
            Assert.Pass();
        }
    }
}
