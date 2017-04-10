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
	}
}