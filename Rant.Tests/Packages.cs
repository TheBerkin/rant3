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
            package.AddResource(RantProgram.CompileString("TestProgram", @"[case:title]<noun-food-fruit-yellow> [rs:5;,\s]{[rn]}"));
            package.Save("TestPackage.rantpkg");

            package = RantPackage.Load("TestPackage.rantpkg");
            rant.LoadPackage(package);

            Assert.AreEqual("Banana 1, 2, 3, 4, 5", rant.DoName("TestProgram").Main);
            Assert.AreEqual("TestPackage", package.ID);
            Assert.AreEqual("This is a test.", package.Description);
            Assert.AreEqual("Test Package?!", package.Title);
            Assert.AreEqual("1.1.0", package.Version.ToString());
        }
    }
}