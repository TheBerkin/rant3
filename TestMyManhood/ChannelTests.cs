using Manhood;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMyManhood
{
    [TestClass]
    public class ChannelTests
    {
        [TestMethod]
        public void PublicPrivateExclusion()
        {
            var mh = new ManhoodContext();
            var cc = mh.Do("Public[out x/private]Private[out x/off]", 0);
            Assert.AreEqual("Public", cc["main"].Output);
            Assert.AreEqual("Private", cc["x"].Output);
        }
    }
}
