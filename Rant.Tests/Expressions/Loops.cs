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
    (function() 
    {
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
    })();
]");
            Assert.AreEqual("this is a test", output.Main);
        }

        [Test]
        public void Fibonacci()
        {
            var output = rant.Do(@"
[@
    {
        var fibonacci = (n) =>
        {
            var a = 0;
            var b = 1;
            var i = 0;
            while(i < n)
            {
                if (i > 0) Output.print("", "");
                Output.print(a);
                var temp = a;
                a = b;
                b += temp;
                i++;
            }
        }
        fibonacci(10);
    }
]
");
            Assert.AreEqual("0, 1, 1, 2, 3, 5, 8, 13, 21, 34", output.Main);
        }
    }
}