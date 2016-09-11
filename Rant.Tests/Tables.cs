using System.IO;

using NUnit.Framework;

using Rant.Vocabulary;

namespace Rant.Tests
{
	[TestFixture]
	[SetCulture("en-US")]
	public class Tables
	{
		[Test]
		public void SimpleNounTable()
		{
			var table = RantDictionaryTable.FromStream("nouns.table", File.Open("Tables/nouns.table", FileMode.Open));
			Assert.AreEqual("noun", table.Name);
			Assert.AreEqual(2, table.TermsPerEntry);
			Assert.AreEqual(0, table.GetSubtypeIndex("singular"));
			Assert.AreEqual(0, table.GetSubtypeIndex("singular"));
			Assert.AreEqual(1, table.GetSubtypeIndex("plural"));
			Assert.AreEqual(1, table.GetSubtypeIndex("p"));
			Assert.AreEqual(-1, table.GetSubtypeIndex("nonexistent"));
		}

		[Test]
		public void VerbTableWithPhrasals()
		{
			var table = RantDictionaryTable.FromStream("verbs.table", File.Open("Tables/verbs.table", FileMode.Open));
			Assert.AreEqual("verb", table.Name);
			Assert.AreEqual(8, table.TermsPerEntry);
		}

		[Test]
		public void Emoji()
		{
			var table = RantDictionaryTable.FromStream("emoji.table", File.Open("Tables/emoji.table", FileMode.Open));
			Assert.AreEqual("emoji", table.Name);
			Assert.AreEqual(1, table.TermsPerEntry);
		}
	}
}