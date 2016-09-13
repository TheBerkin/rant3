using System.IO;

using NUnit.Framework;

using Rant.Resources;
using Rant.Vocabulary;

namespace Rant.Tests
{
	[TestFixture]
	public class Packages
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		public void SaveLoadRun()
		{
			var package = new RantPackage
			{
				ID = "TestPackage",
				Description = "This is a test.",
				Title = "Test Package?!",
				Version = new RantPackageVersion(1, 1, 0)
			};

			package.AddResource(RantDictionaryTable.FromStream("nouns", File.Open("Tables/nouns.table", FileMode.Open)));
			package.AddResource(RantProgram.CompileString("TestProgram", @"<noun-fruit-long-yellow>"));
			package.Save("TestPackage.rantpkg");
			
			rant.LoadPackage("TestPackage.rantpkg");
			
			Assert.AreEqual("banana", rant.DoPackaged("TestProgram").Main);	
		}
	}
}