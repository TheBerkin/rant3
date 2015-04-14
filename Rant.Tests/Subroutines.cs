using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class Subroutines
	{
		[Test]
		public void NoParamsOutput()
		{
			Assert.AreEqual("1 2 3 4 5 6 7 8 9 10", 
				new RantEngine().Do(@"[$[test]:[r:10][s:\s]{[rn]}][$test]").MainValue);
		}

		[Test]
		[ExpectedException(typeof(RantRuntimeException))]
		public void OutOfScope()
		{
			new RantEngine().Do(@"{[$[test]:\16,c]}[$test]");
		}

		[Test]
		public void InnerScope()
		{
			new RantEngine().Do(@"[$[test]:\16,c]{[$test]}");
		}

		[Test]
		[ExpectedException(typeof(RantRuntimeException))]
		public void MissingSubroutine()
		{
			new RantEngine().Do(@"[$missing]");
		}
	}
}