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
        [TestCase(9, 2, "%", "1")]
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
            Assert.AreEqual("true", rant.Do("[@ { x = 4; } x == ??? ]").Main);
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

        [Test]
        [TestCase("<", "5", "2 + 5", "true")]
        [TestCase(">", "5 + 2", "7 + 4", "false")]
        [TestCase("<=", "5", "2 + 5", "true")]
        [TestCase(">=", "5 + 2", "7 + 4", "false")]
        public void ComparisonRightAssociative(string op, string x, string y, string result)
        {
            Assert.AreEqual(result, rant.Do($"[@ {x} {op} {y} ]").Main);
        }

        [Test]
        public void InverseOperator()
        {
            Assert.AreEqual("false", rant.Do("[@ !(true) ]").Main);
        }

        [Test]
        public void PrefixIncrementOperator()
        {
            Assert.AreEqual("6", rant.Do("[@ x = 5; ++x; x ]").Main);
        }

        [Test]
        public void PrefixDecrementOperator()
        {
            Assert.AreEqual("4", rant.Do("[@ x = 5; --x; x ]").Main);
        }

        [Test]
        public void PostfixIncrementOperator()
        {
            Assert.AreEqual("6", rant.Do("[@ x = 5; x++ ]").Main);
        }

        [Test]
        public void PostfixDecrementOperator()
        {
            Assert.AreEqual("4", rant.Do("[@ x = 5; x-- ]").Main);
        }

        [Test]
        public void BooleanAndOperator()
        {
            Assert.AreEqual("true", rant.Do("[@ true && true ]").Main);
        }

        [Test]
        public void BooleanOrOperator()
        {
            Assert.AreEqual("true", rant.Do("[@ false || true ]").Main);
        }

        [Test]
        public void SingleLineComments()
        {
            Assert.AreEqual(string.Empty, rant.Do("[@ # test \n]").Main);
        }
    }
}
