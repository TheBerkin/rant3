using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class Variables
	{
		[Test]
		public void VariableExists()
		{
			var rant = new RantEngine();
			rant.Do("[vn:testvar;123]");
			Assert.IsNotNull(rant["testvar"]);
		}

		[Test]
		public void VariableDoesNotExist()
		{
			var rant = new RantEngine();
			Assert.IsNull(rant["testvar"]);
		}

		[Test]
		public void PrintNumericVariable()
		{
			var rant = new RantEngine();
			Assert.AreEqual("123", rant.Do("[vn:a;123][v:a]").Main);
		}

		[Test]
		public void PrintStringVariable()
		{
			var rant = new RantEngine();
			Assert.AreEqual("Example", rant.Do("[vs:a;Example][v:a]").Main);
		}

		[Test]
		public void AddNumbers()
		{
			var rant = new RantEngine();
			Assert.AreEqual("100", rant.Do("[vn:a;75][vn:b;25][add:[v:a];[v:b]]").Main);
		}

		[Test]
		public void AddStrings()
		{
			var rant = new RantEngine();
			Assert.AreEqual("FooBar", rant.Do("[vs:a;Foo][vs:b;Bar][v:a][v:b]").Main);
		}

		[Test]
		public void PatternVariable()
		{
			var rant = new RantEngine();
			Assert.AreEqual("AB", rant.Do("[vp:a;[x:_;forward]{A|B}][v:a][v:a]").Main);
		}

		[Test]
		public void VariableScope()
		{
			var rant = new RantEngine();
			rant.Do("{[vn:a;100]}[vn:b;150]");
			Assert.IsNull(rant["a"]);
			Assert.IsNotNull(rant["b"]);
		}

		[Test]
		public void VariableSwap()
		{
			var rant = new RantEngine();
			rant.Do("[vn:a;3][vn:b;5][swap:a;b]");
			Assert.AreEqual(rant["a"].Value, 5);
			Assert.AreEqual(rant["b"].Value, 3);
		}

		[Test]
		public void ListFilter()
		{
			var rant = new RantEngine();
			var result = rant.Do(@"
				[vl:test][laddn:test;1][laddn:test;2][laddn:test;3][laddn:test;4]
				[lfilter:test;a;[gt:[v:a];2]][v:test]").Main;
			Assert.AreEqual("(3, 4)", result);
		}

		[Test]
		public void ListMap()
		{
			var rant = new RantEngine();
			var result = rant.Do(@"
				[vl:test][laddn:test;1][laddn:test;2][laddn:test;3][laddn:test;4]
				[lmap:test;a;[v:a][v:a]][v:test]").Main;
			Assert.AreEqual("(11, 22, 33, 44)", result);
		}

		[Test]
		public void StringSplit()
		{
			var rant = new RantEngine();
			var result = rant.Do("[split:a;,;\"a,b,c,d\"][v:a]").Main;
			Assert.AreEqual("(a, b, c, d)", result);
		}

		[Test]
		public void Loop()
		{
			var rant = new RantEngine();
			var result = rant.Do("[vn:a;10][while:[gt:[v:a];0];[vsub:a;1]a]").Main;
			Assert.AreEqual(result, "aaaaaaaaaa");
		}
	}
}