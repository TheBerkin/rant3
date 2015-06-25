using System;
using System.Linq;

using NUnit.Framework;

using Rant.Engine;
using Rant.Engine.Formatters;

namespace Rant.Tests
{
	[TestFixture]
	public class Utilities
	{
		[Test]
		public void FunctionExists()
		{
			Assert.IsTrue(RantUtils.FunctionExists("rep")); // Inherited function name
			Assert.IsTrue(RantUtils.FunctionExists("r"));	// Alias
		}

		[Test]
		public void FunctionDescription()
		{
			Assert.AreEqual("Sets the repetition count for the next block.", RantUtils.GetFunctionDescription("rep", 1));
		}

	    [Test]
	    public void SnakeToCamel()
	    {
	        Assert.AreEqual("LoremIpsum", Util.SnakeToCamel("lorem-ipsum"));
	    }

	    [Test]
	    public void CamelToSnake()
	    {
	        Assert.AreEqual("lorem-ipsum", Util.CamelToSnake("LoremIpsum"));
            Assert.AreEqual("url-encoding", Util.CamelToSnake("URLEncoding"));
            Assert.AreEqual("invalid-crc", Util.CamelToSnake("InvalidCRC"));
            Assert.AreEqual("use-html-daily", Util.CamelToSnake("UseHTMLDaily"));
            Assert.AreEqual("something", Util.CamelToSnake("Something"));
            Assert.AreEqual("something", Util.CamelToSnake("SOMETHING"));
	    }
	}
}