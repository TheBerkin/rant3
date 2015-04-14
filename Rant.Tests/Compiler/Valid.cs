using System;

using NUnit.Framework;

namespace Rant.Tests.Compiler
{
	[TestFixture]
	public class Valid
	{
		[Test]
		public void Plaintext()
		{
			RantPattern.FromString(@"just some text");
		}

		[TestCase(@"{}")]
		[TestCase(@"{|}")]
		[TestCase(@"{||}")]
		[TestCase(@"{Item 1}")]
		[TestCase(@"{Item 1|Item 2}")]
		[TestCase(@"{Item 1|Item 2|Item 3}")]
		public void Blocks(string pattern)
		{
			RantPattern.FromString(pattern);
		}

		[Test]
		public void SubroutineNoParams()
		{
			RantPattern.FromString(@"[$[test]:{A|B|C|D}]");
		}

		[TestCase("arg1")]
		[TestCase("@arg1")]
		[TestCase("arg1;arg2")]
		[TestCase("@arg1;arg2")]
		[TestCase("arg1;arg2;arg3")]
		[TestCase("@arg1;@arg2;@arg3")]
		public void SubroutineParams(string args)
		{
			RantPattern.FromString($"[$[test:{args}]:{{A|B|C|D}}]");
		}
	}
}