using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rant.Tests
{
	[TestFixture]
	internal class Queries
	{
		private readonly RantEngine rant = new RantEngine();

		[Test]
		public void Basic()
		{
			Assert.IsNotNull(rant.Do("<noun>"));
		}
	}
}
