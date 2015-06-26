using NUnit.Framework;

namespace Rant.Tests.Expressions
{
    [TestFixture]
    public class Loops
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void StringBuildingWhileLoop()
        {
            var output = rant.Do(@"
[@
    var parts = (""this"", ""is"", ""a"", ""test"");
    var i = 0;
    var buffer = """";
    while(i < parts.length)
    {
        if (i > 0) buffer ~= "" "";
        buffer ~= parts[i];
        i++;
    }
    return buffer;
]");
            Assert.AreEqual("this is a test", output.Main);
        }
    }
}