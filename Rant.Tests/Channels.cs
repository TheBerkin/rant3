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
			var output = rant.Do(@"Hello [chan:foo;public;World]");
			Assert.AreEqual("Hello World", output.Main);
			Assert.AreEqual("World", output["foo"]);
		}

		[Test]
		public void Internal()
		{
			var output = rant.Do(@"Public Text[chan:foo;internal;[chan:bar;internal;Internal Text]]");
			Assert.AreEqual("Public Text", output.Main);
			Assert.AreEqual("Internal Text", output["foo"]);
			Assert.AreEqual("Internal Text", output["bar"]);
		}

		[Test]
		public void Private()
		{
			var output = rant.Do(@"Public Text[chan:secret;private;Private Text]");
			Assert.AreEqual("Public Text", output.Main);
			Assert.AreEqual("Private Text", output["secret"]);
		}

		[Test]
		public void NestedPrivate()
		{
			var output = rant.Do(@"Public Text[chan:secret_1;private;Private Text 1[chan:secret_2;private;Private Text 2]]");
			Assert.AreEqual("Public Text", output.Main);
			Assert.AreEqual("Private Text 1", output["secret_1"]);
			Assert.AreEqual("Private Text 2", output["secret_2"]);
		}

		[Test]
		public void ClosedPrivate()
		{
			var output = rant.Do(@"foo[chan:secret;private;]bar");
			Assert.AreEqual("foobar", output.Main);
			Assert.AreEqual(String.Empty, output["secret"]);
		}

		[Test]
		public void PrivateOnly()
		{
			var output = rant.Do(@"[chan:secret;private;Private Text]");
			Assert.AreEqual(String.Empty, output.Main);
			Assert.AreEqual("Private Text", output["secret"]);
		}

		[Test]
		public void InternalOnly()
		{
			var output = rant.Do(@"[chan:secret;internal;Internal Text]");
			Assert.AreEqual(String.Empty, output.Main);
			Assert.AreEqual("Internal Text", output["secret"]);
		}

		[Test]
		public void PrivateInternalBlock()
		{
			var output = rant.Do(
				@"Public Text[chan:internal_a;internal;[chan:secret;private;Private/[chan:internal_b;internal;Internal Text]]]");
			Assert.AreEqual("Public Text", output.Main);
			Assert.AreEqual(String.Empty, output["internal_a"]);
			Assert.AreEqual("Private/Internal Text", output["secret"]);
			Assert.AreEqual("Internal Text", output["internal_b"]);
		}

		[Test]
		public void InternalPrivate()
		{
			var output = rant.Do(@"Public Text[chan:foo;internal;[chan:bar;internal;Internal Text[chan:secret;private;Private Text]]]");
			Assert.AreEqual("Public Text", output.Main);
			Assert.AreEqual("Internal Text", output["foo"]);
			Assert.AreEqual("Internal Text", output["bar"]);
			Assert.AreEqual("Private Text", output["secret"]);
		}
	}
}