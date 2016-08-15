using System;
using System.Linq;

using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class Blocks
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		public void ConstantWeights()
		{
			Assert.AreEqual("bbbbbbbbbb", rant.Do(@"[r:10]{(0)a|b}").Main);
		}

		[Test]
		public void BasicRepeater()
		{
			Assert.AreEqual("blahblahblahblahblah",
				rant.Do(@"[r:5]{blah}").Main);
		}

		[Test]
		public void NestedRepeaters()
		{
			Assert.AreEqual("ABBBABBBABBB",
				rant.Do(@"[r:3]{A[r:3]{B}}").Main);
		}

		[Test]
		public void RepeaterSeparator()
		{
			Assert.AreEqual("1, 2, 3, 4, 5",
				rant.Do(@"[r:5][s:,\s]{[rn]}").Main);
		}

		[Test]
		public void RepeaterSeparatorNested()
		{
			Assert.AreEqual("(1), (1 2), (1 2 3), (1 2 3 4), (1 2 3 4 5)",
				rant.Do(@"[rs:5;,\s][before:(][after:)]{[rs:[rn];\s]{[rn]}}").Main);
		}

		[Test]
		public void BlockDepth()
		{
			Assert.AreEqual("0, 1, 2, 3",
				rant.Do(@"[depth], {[depth]}, {{[depth]}}, {{{[depth]}}}").Main);
		}

		[Test]
		public void LockedSynchronizer()
		{
			var output =
				rant.Do(@"[r:100][x:_;locked]{A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|1|2|3|4|5|6|7|8|9|0}", seed: 0)
					.Main;
			Console.WriteLine(output);
			Assert.IsTrue(output.Distinct().Count() == 1);
		}

		[Test]
		public void SeriesCommas()
		{
			Assert.AreEqual("dogs, dogs, dogs and dogs",
				rant.Do(@"[r:4][s:,;and]{dogs}").Main);
		}

		[Test]
		public void OxfordComma()
		{
			Assert.AreEqual("dogs, dogs, dogs, and dogs",
				rant.Do(@"[r:4][s:,;,;and]{dogs}").Main);
		}

		[Test]
		public void FormattedOxfordSeries()
		{
			Assert.AreEqual("Dogs, dogs, dogs, dogs, and dogs. Cats are also pretty neat.",
				rant.Do(@"[case:sentence][r:5][s:,;,;and]{dogs}. cats are also pretty neat.").Main);
		}

		[Test]
		public void EnumerateItems()
		{
			Assert.AreEqual("ABCDEFGH", 
				rant.Do(@"[repeach][x:_;ordered]{A|B|C|D|E|F|G|H}").Main);
		}
    }
}