using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class StringOps
	{
		private readonly RantEngine rant = new RantEngine();

		[TestCase("abc", "cba")]
		[TestCase("The symbol 'e\u030a\u0321' is actually three characters", "sretcarahc eerht yllautca si 'e\u030a\u0321' lobmys ehT")]
		[TestCase("Text with \U0001f602 emoji", "ijome \U0001f602 htiw txeT")]
		public void Reverse(string original, string reverse)
		{
			Assert.AreEqual(reverse, rant.Do($"[rev:{original}]").Main);
		}

		[TestCase(@"(abc)", "(cba)")]
		[TestCase(@"\[abc\]", "[cba]")]
		[TestCase(@"\{abc\}", "{cba}")]
		[TestCase(@"(\[\{abc\}\])", "([{cba}])")]
		public void ReverseEx(string original, string reverse)
		{
			Assert.AreEqual(reverse, rant.Do($"[revx:{original}]").Main);
		}
		
		[TestCase(@"abc", 2, "c")]
		[TestCase(@"abc", 1, "b")]
		[TestCase(@"abc", 0, "a")]
		public void StringAt(string full, int pos, string result)
		{
			Assert.AreEqual(result, rant.Do($"[at:{full};{pos}]").Main);
		}

		[Test]
		[ExpectedException(typeof(RantRuntimeException))]
		public void StringAtException()
		{
			rant.Do("[at:abc;-1]");
		}
	}
}