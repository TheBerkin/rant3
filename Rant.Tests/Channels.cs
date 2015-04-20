using System;

using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class Channels
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		public void Public()
		{
			var output = rant.Do(@"Hello [open:foo;public]World");
			Assert.AreEqual("Hello World", output.MainValue);
			Assert.AreEqual("World", output["foo"]?.Value);
		}

		[Test]
		public void NestedPublic()
		{
			var output = rant.Do(@"1 [open:a;public]2 [open:b;public]3");
			Assert.AreEqual("1 2 3", output.MainValue);
			Assert.AreEqual("2 3", output["a"]?.Value);
			Assert.AreEqual("3", output["b"]?.Value);
		}

		[Test]
		public void Internal()
		{
			var output = rant.Do(@"Public Text[open:foo;internal][open:bar;internal]Internal Text");
			Assert.AreEqual("Public Text", output.MainValue);
			Assert.AreEqual("Internal Text", output["foo"]?.Value);
			Assert.AreEqual("Internal Text", output["bar"]?.Value);
		}

		[Test]
		public void Private()
		{
			var output = rant.Do(@"Public Text[open:secret;private]Private Text");
			Assert.AreEqual("Public Text", output.MainValue);
			Assert.AreEqual("Private Text", output["secret"]?.Value);
		}

		[Test]
		public void NestedPrivate()
		{
			var output = rant.Do(@"Public Text[open:secret_1;private]Private Text 1[open:secret_2;private]Private Text 2");
			Assert.AreEqual("Public Text", output.MainValue);
			Assert.AreEqual("Private Text 1", output["secret_1"]?.Value);
			Assert.AreEqual("Private Text 2", output["secret_2"]?.Value);
		}

		[Test]
		public void ClosedPrivate()
		{
			var output = rant.Do(@"foo[open:secret;private][close:secret]bar");
			Assert.AreEqual("foobar", output.MainValue);
			Assert.AreEqual(String.Empty, output["secret"]?.Value);
		}

		[Test]
		public void PrivateOnly()
		{
			var output = rant.Do(@"[open:secret;private]Private Text");
			Assert.AreEqual(String.Empty, output.MainValue);
			Assert.AreEqual("Private Text", output["secret"]?.Value);
		}

		[Test]
		public void InternalOnly()
		{
			var output = rant.Do(@"[open:secret;internal]Internal Text");
			Assert.AreEqual(String.Empty, output.MainValue);
			Assert.AreEqual("Internal Text", output["secret"]?.Value);
		}

		[Test]
		public void PrivateInternalBlock()
		{
			var output = rant.Do(
				@"Public Text[open:internal_a;internal][open:secret;private]Private/[open:internal_b;internal]Internal Text");
			Assert.AreEqual("Public Text", output.MainValue);
			Assert.AreEqual(String.Empty, output["internal_a"]?.Value);
			Assert.AreEqual("Private/Internal Text", output["secret"]?.Value);
			Assert.AreEqual("Internal Text", output["internal_b"].Value);
		}

		[Test]
		public void PrivateInternalPass()
		{
			var output = rant.Do(
				@"Public Text[open:internal_a;internal][open:secret;private]Private Text[open:internal_b;internal][close:secret]Internal Text");
			Assert.AreEqual("Public Text", output.MainValue);
			Assert.AreEqual("Internal Text", output["internal_a"]?.Value);
			Assert.AreEqual("Private Text", output["secret"]?.Value);
			Assert.AreEqual("Internal Text", output["internal_b"].Value);
		}

		[Test]
		public void InternalPrivate()
		{
			var output = rant.Do(@"Public Text[open:foo;internal][open:bar;internal]Internal Text[open:secret;private]Private Text");
			Assert.AreEqual("Public Text", output.MainValue);
			Assert.AreEqual("Internal Text", output["foo"]?.Value);
			Assert.AreEqual("Internal Text", output["bar"]?.Value);
			Assert.AreEqual("Private Text", output["secret"]?.Value);
		}
	}
}