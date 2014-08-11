using Manhood;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMyManhood
{
    [TestClass]
    public class Runtime
    {
        [TestMethod]
        public void CommentExclusion()
        {
            var mh = new ManhoodContext();

            Assert.AreEqual("Hello World!", mh.Do(@"[isub comment/\`\`This should not show up.\`\`]Hello World![$comment]"));
        }
    }
}