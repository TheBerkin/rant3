using System.IO;
using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class Modules
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		public void FileModules()
		{
			File.WriteAllText("rant_module_test.module.rant", "[$[.hey]:Hello, World!]");
			Assert.AreEqual("Hello, World!", rant.Do("[use:rant_module_test][$rant_module_test.hey]").Main);
			File.Delete("rant_module_test.module.rant");
		}

		[Test]
		public void UserModules()
		{
			var module = new RantModule("user_module");
			module.AddSubroutineFunction("test", RantPattern.FromString("[$[.test]:A Good Test]"));
			rant.Modules["user_module"] = module;
			Assert.AreEqual("A Good Test", rant.Do("[use:user_module][$user_module.test]").Main);
		}
	}
}
