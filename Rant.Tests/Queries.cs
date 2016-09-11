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
	}
}
