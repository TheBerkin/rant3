using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Rant.Tests.Richard.StdLib
{
    [TestFixture]
    public class StdMath
    {
        private readonly RantEngine rant = new RantEngine();

        [Test]
        public void MathPI()
        {
            Assert.AreEqual(Math.PI.ToString(), rant.Do("[@ Math.PI ]").Main);
        }

        [Test]
        public void MathE()
        {
            Assert.AreEqual(Math.E.ToString(), rant.Do("[@ Math.E ]").Main);
        }

        [Test]
        public void MathAtan2()
        {
            Assert.AreEqual(Math.Atan2(1, 4).ToString(), rant.Do("[@ Math.atan2(1, 4) ]").Main);
        }

        [Test]
        public void MathPow()
        {
            Assert.AreEqual("9", rant.Do("[@ Math.pow(3, 2); ]").Main);
        }

        private static readonly Dictionary<string, Func<double, double>> testCases =
            new Dictionary<string, Func<double, double>>()
        {
                { "acos", Math.Acos },
                { "asin", Math.Asin },
                { "atan", Math.Atan },
                { "ceil", Math.Ceiling },
                { "cos", Math.Cos },
                { "cosh", Math.Cosh },
                { "exp", Math.Exp },
                { "floor", Math.Floor },
                { "log", Math.Log },
                { "log10", Math.Log10 },
                { "round", Math.Round },
                { "sin", Math.Sin },
                { "sinh", Math.Sinh },
                { "sqrt", Math.Sqrt },
                { "tan", Math.Tan },
                { "tanh", Math.Tanh },
                { "truncate", Math.Truncate }
        };

        [Test]
        [TestCase("acos")]
        [TestCase("asin")]
        [TestCase("atan")]
        [TestCase("ceil")]
        [TestCase("cos")]
        [TestCase("cosh")]
        [TestCase("exp")]
        [TestCase("floor")]
        [TestCase("log")]
        [TestCase("log10")]
        [TestCase("round")]
        [TestCase("sin")]
        [TestCase("sinh")]
        [TestCase("sqrt")]
        [TestCase("tan")]
        [TestCase("tanh")]
        [TestCase("truncate")]
        public void MathMethods(string action)
        {
            var method = testCases[action];
            var result = method(1.5);
            Assert.AreEqual(result.ToString(), rant.Do($"[@ Math.{action}(1.5) ]").Main);
        }
    }
}
