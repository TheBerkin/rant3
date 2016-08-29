using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class StringOps
	{
		private readonly RantEngine rant = new RantEngine();

		[TestCase("abc", "cba")]
		[TestCase("The symbol 'e\u030a\u0321' is actually three characters", "sretcarahc eerht yllautca si 'e\u030a\u0321' lobmys ehT")]
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
	}
}