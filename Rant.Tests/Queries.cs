using System.IO;

using NUnit.Framework;

using Rant.Vocabulary;

namespace Rant.Tests
{
	[TestFixture]
	internal class Queries
	{
		private readonly RantEngine rant = new RantEngine();

		public Queries()
		{
			rant.Dictionary.AddTable(RantDictionaryTable.FromStream("verbs", File.Open("Tables/verbs.table", FileMode.Open)));
			rant.Dictionary.AddTable(RantDictionaryTable.FromStream("nouns", File.Open("Tables/nouns.table", FileMode.Open)));
			rant.Dictionary.AddTable(RantDictionaryTable.FromStream("nsfw", File.Open("Tables/nsfw.table", FileMode.Open)));
		}

		[Test]
		public void Basic()
		{
			Assert.IsNotNull(rant.Do("<noun>"));
		}

		[Test]
		public void PhrasalVerb()
		{
			Assert.AreEqual("It really pisses him off.", rant.Do(@"It really <verb.s-phrasal_test[him]>.").Main);
			Assert.AreEqual("Whom will he piss off today?", rant.Do(@"Whom will he <verb-phrasal_test> today?").Main);
		}

		[TestCase(@"<noun-unhidden_class>", "[No Match]")]
		[TestCase(@"<nsfw>", "[No Match]")]
		[TestCase(@"<noun-nsfw>", "butt")]
		public void HiddenClass(string pattern, string expected)
		{
			Assert.AreEqual(expected, rant.Do(pattern).Main);
		}

		[Test]
		public void UnhiddenClass()
		{
			rant.Dictionary.IncludeHiddenClass("nsfw");
			Assert.AreEqual("butt", rant.Do(@"<noun-unhidden_class>").Main);
			rant.Dictionary.ExcludeHiddenClass("nsfw");
			Assert.AreEqual("[No Match]", rant.Do(@"<noun-unhidden_class>").Main);
		}
	}
}
