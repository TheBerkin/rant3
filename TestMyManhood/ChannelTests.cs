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
            ManhoodContext mh = new ManhoodContext();
            var cc = mh.Do("Public[ch x/private]Private[ch x/off]", 0);
            Assert.AreEqual("Public", cc["main"].Output);
            Assert.AreEqual("Private", cc["x"].Output);
        }
    }
}
