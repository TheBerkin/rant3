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
            return;
        }
        fibonacci(10);
    }
]
");
            Assert.AreEqual("0, 1, 1, 2, 3, 5, 8, 13, 21, 34", output.Main);
        }

        [Test]
        public void WhereSelector()
        {
            var output = rant.Do(@"
[@{
  var Select = 
  {
    # filter(condition(current))
    where: (lst, filter) => 
    {
      var results = [];
      for(var i in lst)
      {
        if (filter(lst[i]))
          results.push(lst[i]);
      }
      return results;
    },
    # accumulator(current, next)
    concat: (lst, accumulator) =>
    {
      var buffer = """";
      for(var i in lst)
      {
        buffer ~= accumulator(buffer, lst[i]);
      }
      return buffer;
    }
  };
  
  return Select.concat(
    Select.where(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 
    (x) => { 
      return x % 2 == 0; 
    }),
    (current, next) => {
      if (current.length > 0) current ~= "", "";
      current ~= next;
    });
}]
");
            Assert.AreEqual("2, 4, 6, 8, 10", output.Main);
        }
    }
}