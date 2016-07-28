using System;

using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class StringLocalization
	{
		[Test]
		public void StaticString()
		{
			var str = Rant.Localization.Txtres.GetString("bad-subtype");
			Console.WriteLine(str);
			Assert.AreNotEqual("bad-subtype", str);
		}
	}
}