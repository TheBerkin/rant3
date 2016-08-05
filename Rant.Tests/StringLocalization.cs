using System;
using System.Globalization;
using System.Threading;

using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	public class StringLocalization
	{
		[Test]
		public void StaticString()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
			var str = Rant.Localization.Txtres.GetString("bad-subtype");
			Assert.AreEqual("[Bad Subtype]", str);
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
			str = Rant.Localization.Txtres.GetString("bad-subtype");
			Assert.AreEqual("[Ungültiger Subtyp]", str);
		}
	}
}