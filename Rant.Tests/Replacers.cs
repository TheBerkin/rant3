using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Rant.Tests
{
    [TestFixture]
    public class Replacers
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void ReplacerOverflow()
        {
            string result = rant.Do("[//x//:[char:x;64];y]");
            Assert.AreEqual("yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", result);
        }

        [Test]
        public void ReplacerNoMatches()
        {
            string result = rant.Do(@"[//\s//:TestString;]");
            Assert.AreEqual(result, "TestString");
        }

        [Test]
        public void VowelReplacer()
        {
            string result = rant.Do("[//[aeiou]//i: The quick brown fox jumps over the lazy dog.; {a|e|i|o|u}]");
            Assert.True(new Regex(@"Th[aeiou] q[aeiou][aeiou]ck br[aeiou]wn f[aeiou]x j[aeiou]mps [aeiou]v[aeiou]r th[aeiou] l[aeiou]zy d[aeiou]g\.").IsMatch(result));
        }

        [Test]
        public void GroupTest()
        {
            string result = rant.Do(@"[//(?<a>(s|\b)(?<b>s)|t(?<b>h)|(?<b>s\b))//i:The slimy snake silently slithers through the leaves.;[group:a][rep:2]{[group:b]}]");
            Assert.AreEqual("Thhhe ssslimy sssnake sssilently ssslithhhersss thhhrough thhhe leavesss.", result);
        }

        [Test]
        public void MatchTest()
        {
            string result = rant.Do(@"[//(?<=\b(\w\w)*)\w(?=\w*)//:Isn't it a lovely afternoon?;[caps:first][match]]");
            Assert.AreEqual("IsN'T It A LoVeLy AfTeRnOoN?", result);
        }
    }
}
