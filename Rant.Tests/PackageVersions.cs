using NUnit.Framework;

using Rant.Resources;

namespace Rant.Tests
{
	[TestFixture]
	public class PackageVersions
	{
		[TestCase("1.2.5", "1.2.4", true)]
		[TestCase("0.2.5", "0.2.4", true)]
		[TestCase("0.0.3", "0.0.4", false)]
		[TestCase("0.3.1", "0.4.0", false)]
		public void VersionGreaterThan(string v1, string v2, bool expactedResult)
		{
			Assert.IsTrue(RantPackageVersion.Parse(v1) > RantPackageVersion.Parse(v2) == expactedResult, $"Expected {expactedResult}");
		}

		[TestCase("2.2.1", "2.2.4", true)]
		[TestCase("3.2.1", "2.1.4", false)]
		[TestCase("0.2.3", "0.2.4", true)]
		[TestCase("1.3.0", "0.5.4", false)]
		public void VersionLessThan(string v1, string v2, bool expectedResult)
		{
			Assert.IsTrue(RantPackageVersion.Parse(v1) < RantPackageVersion.Parse(v2) == expectedResult, $"Expected {expectedResult}");
		}
	}
}