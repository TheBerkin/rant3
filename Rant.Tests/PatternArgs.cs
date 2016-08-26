using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class PatternArgs
	{
		private readonly RantEngine rant = new RantEngine();

		private class TestCustomArgsClass
		{
			[RantArg("word-a")]
			public string A { get; set; }

			[RantArg("word-b")]
			public string B { get; set; }
		}

		[Test]
		public void OneArg()
		{
			var args = new RantProgramArgs
			{
				["foo"] = "Bar"
			};
			Assert.AreEqual("FooBar", rant.Do(@"Foo[in:foo]", 0, 0, args).Main);
		}

		[Test]
		public void OneArgAnonymousType()
		{
			var args = new
			{
				msg = "Hello World"
			};

			Assert.AreEqual("Hello World", rant.Do(@"[in:msg]", 0, 0, RantProgramArgs.CreateFrom(args)).Main);
		}

		[Test]
		public void CustomArgsClass()
		{
			var args = new TestCustomArgsClass
			{
				A = "Hello",
				B = "World"
			};

			Assert.AreEqual("Hello World!", rant.Do(@"[in:word-a] [in:word-b]!", 0, 0, RantProgramArgs.CreateFrom(args)).Main);
		}
	}
}