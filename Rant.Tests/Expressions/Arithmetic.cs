using NUnit.Framework;

namespace Rant.Tests.Expressions
{
	[TestFixture]
	public class Arithmetic
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		[TestCase(2, 3, "+", "5")]
		[TestCase(5, 1, "-", "4")]
		[TestCase(4, 2, "*", "8")]
		[TestCase(8, 4, "/", "2")]
		public void BasicOperators(int a, int b, string op, string result)
		{
			Assert.AreEqual(result, rant.Do($"[@ {a} {op} {b} ]").Main);
		}

		[Test]
        [TestCase("(1 + 2) * 3 + 4", "13")]
		[TestCase("2 + 2 / 2", "3")]
		[TestCase("(2 + 2) / 2", "2")]
		public void OrderOfOperations(string expr, string result)
		{
			Assert.AreEqual(result, rant.Do($"[@ {expr} ]").Main);
        }

        [Test]
        public void VariableAssignment()
        {
            Assert.AreEqual("2", rant.Do("[@ x = 2; x ]").Main);
        }

        [Test]
        public void VariableScoping()
        {
            Assert.AreEqual("no", rant.Do("[@ { x = 4; } x ]").Main);
        }

        [Test]
        public void VarVariableScoping()
        {
            Assert.AreEqual("4", rant.Do("[@ var x; { x = 4; }; x ]").Main);
        }

        [Test]
        [TestCase("10", "+")]
        [TestCase("0", "-")]
        [TestCase("25", "*")]
        [TestCase("1", "/")]
        public void AssignmentOperators(string result, string op)
        {
            Assert.AreEqual(result, rant.Do($"[@ x = 5; x {op}= 5; x ]").Main);
        }
	}
}
