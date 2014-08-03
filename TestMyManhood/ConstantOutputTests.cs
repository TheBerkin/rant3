using Manhood;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMyManhood
{
    [TestClass]
    public class ConstantOutputTests
    {
        [TestMethod]
        public void BlockSingleOption()
        {
            ManhoodContext mh = new ManhoodContext();
            Assert.AreEqual("test my manhood", mh.Do("{test my manhood}", 0));
        }

        [TestMethod]
        public void UppercaseBlockSingleOption()
        {
            ManhoodContext mh = new ManhoodContext();
            Assert.AreEqual("TEST MY MANHOOD", mh.Do("[caps upper]{test my manhood}", 0));
        }

        [TestMethod]
        public void ProperBlockSingleOption()
        {
            ManhoodContext mh = new ManhoodContext();
            Assert.AreEqual("Test My Manhood", mh.Do("[caps proper]{test my manhood}", 0));
        }

        [TestMethod]
        public void EscapeSequences()
        {
            ManhoodContext mh = new ManhoodContext();
            Assert.AreEqual("\r\n", mh.Do(@"\r\n", 0));
        }
    }
}
