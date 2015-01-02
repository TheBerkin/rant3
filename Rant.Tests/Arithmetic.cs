using System.Globalization;

using NUnit.Framework;

namespace Rant.Tests
{
    [TestFixture]
    public class Arithmetic
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void Constant()
        {
            Assert.AreEqual(rant.Do(@"`1`").MainValue, "1");
        }

        [TestCase(1, 1)]
        [TestCase(3, 2.5)]
        [TestCase(9.1, 5.6)]
        public void Addition(double a, double b)
        {
            Assert.AreEqual(rant.Do("`\{a} + \{b}`").MainValue, (a + b).ToString(CultureInfo.InvariantCulture));
        }

        [TestCase(1, 1)]
        [TestCase(3, 2.5)]
        [TestCase(9.1, 5.6)]
        public void Subtraction(double a, double b)
        {
            Assert.AreEqual(rant.Do("`\{a} - \{b}`").MainValue, (a - b).ToString(CultureInfo.InvariantCulture));
        }

        [TestCase(2, 3)]
        [TestCase(-5, 5)]
        [TestCase(10, 10)]
        public void Multiplication(double a, double b)
        {
            Assert.AreEqual(rant.Do("`\{a} * \{b}`").MainValue, (a * b).ToString(CultureInfo.InvariantCulture));
        }

        [TestCase(100, 10)]
        [TestCase(12, 3)]
        [TestCase(1, 2)]
        public void Division(double a, double b)
        {
            Assert.AreEqual(rant.Do("`\{a} / \{b}`").MainValue, (a / b).ToString(CultureInfo.InvariantCulture));
        }

        [TestCase("7.5 - 0.25 * 2", 7)]
        [TestCase("2 + 3 * 4 / 6", 4)]
        [TestCase("2 * 5 ^ 2", 50)]
        [TestCase("2 * (1 + 3)", 8)]
        [TestCase("(36 - 11) / (2 + 3)", 5)]
        public void OperatorPrecedence(string expression, double expected)
        {
            Assert.AreEqual(rant.Do("`\{expression}`").MainValue, expected.ToString(CultureInfo.InvariantCulture));
        }
    }
}