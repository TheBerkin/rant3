using NUnit.Framework;

namespace Rant.Tests.Richard
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
        buffer = accumulator(buffer, lst[i]);
      }
      return buffer;
    }
  };
  
  return Select.concat(
    Select.where([1, 2, 3, 4, 5, 6, 7, 8, 9, 10], 
    (x) => { 
      return x % 2 == 0; 
    }),
    (current, next) => {
      if (current.length > 0) current ~= "", "";
      current ~= next;
      return current;
    });
}]
");
            Assert.AreEqual("2, 4, 6, 8, 10", output.Main);
        }

        [Test]
        public void BubbleSort()
        {
            var output = rant.Do(@"
[x:_;ordered]
[repeach][sep:,\s]
[@{
  bubbleSort = (lst) =>
  {
    var done = false;
    while (!done)
    {
      done = true;
      var i = 1;
      while(i < lst.length)
      {
        if (lst[i - 1] > lst[i]) 
        {
          done = false;
          var tmp = lst[i - 1];
          lst[i - 1] = lst[i];
          lst[i] = tmp;
        }
        i++;
      }
    }
    return lst;
  };
  return bubbleSort([2, 9, 5, 0, 7, 3, 4, 1, 6, 8]);
}]
");
            Assert.AreEqual("0, 1, 2, 3, 4, 5, 6, 7, 8, 9", output.Main);
        }

        [Test]
        public void Levenshtein()
        {
            var output = rant.Do(@"
[x:_;ordered]
[repeach][sep:,\s]
[@{
  var levenshtein = (str1, str2) =>
  {
      var from = (a, b, func) => {
        while(a <= b)
        {
          func(a++);
        }
      };
      var min = (a, b) => {
        if (a < b) return a;
        return b;
      };
      var m = str1.length;
      var n = str2.length;
      var d = [];
   
      if (m == 0) return n;
      if (n == 0) return m;
   
      from(0, m, (x) => d[x] = [x]);
      from(0, n, (x) => d[0][x] = x);
      
      from(1, n, (j) => {
        from(1, m, (i) => {
          if (str1[i-1] == str2[j-1])
          {
            d[i][j] = d[i - 1][j - 1];
          }
          else
          {
            d[i][j] = min(d[i-1][j], d[i][j-1], d[i-1][j-1]) + 1;
          }          
        });
      });
      return d[m][n];
  };
  return [levenshtein(""hello"", ""hello""), levenshtein(""abc"", ""abb""), levenshtein(""a"", ""aaa""), levenshtein(""foo"", ""bar"")];
}]
");
            Assert.AreEqual("0, 1, 2, 3", output.Main);
        }
    }
}