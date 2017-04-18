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
			RantProgram.CompileString(@"just some text");
		}

		[TestCase(@"{}")]
		[TestCase(@"{|}")]
		[TestCase(@"{||}")]
		[TestCase(@"{Item 1}")]
		[TestCase(@"{Item 1|Item 2}")]
		[TestCase(@"{Item 1|Item 2|Item 3}")]
		public void Blocks(string pattern)
		{
			RantProgram.CompileString(pattern);
		}

		[Test]
		public void SubroutineNoParams()
		{
			RantProgram.CompileString(@"[$[test]:{A|B|C|D}]");
		}

		[TestCase("arg1")]
		[TestCase("@arg1")]
		[TestCase("arg1;arg2")]
		[TestCase("@arg1;arg2")]
		[TestCase("arg1;arg2;arg3")]
		[TestCase("@arg1;@arg2;@arg3")]
		public void SubroutineParams(string args)
		{
			RantProgram.CompileString($"[$[test:{args}]:{{A|B|C|D}}]");
		}

		[TestCase(@"<noun>")]
		[TestCase(@"< noun >")]
		[TestCase(@"< noun.plural >")]
		[TestCase(@"<noun.plural>")]
		[TestCase(@"<noun-class>")]
		[TestCase(@"<noun - class>")]
		[TestCase(@"<noun-class1-class2>")]
		[TestCase(@"<noun-class1-!class2>")]
		[TestCase(@"<noun-!class-!class2>")]
		[TestCase(@"<noun-class1|class2>")]
		[TestCase(@"<noun-class1 | class2>")]
		[TestCase(@"<noun-!class1|!class2>")]
		[TestCase(@"<noun-class1|class2-class3|class4>")]
		[TestCase(@"<noun-!class1|!class2-!class3|!class4>")]
		[TestCase(@"<noun$-class.plural>")]
		[TestCase(@"<noun$-class .plural>")]
		[TestCase(@"<noun$-class>")]
		[TestCase(@"<noun$>")]
		[TestCase(@"<noun ? `regex`>")]
		[TestCase(@"<noun ?! `regex`>")]
		[TestCase(@"<noun ? `regex` ?! `regex`>")]
		[TestCase(@"<noun::=a>")]
		[TestCase(@"<noun :: =a>")]
		[TestCase(@"<noun :: =a @b>")]
		[TestCase(@"<verb[a duck]>")]
		[TestCase(@"<verb[\a <adj> duck]>")]
		[TestCase(@"<verb.ing[\a <adj> <noun>]>")]
		[TestCase(@"<verb[the <noun> <verb.ing[the <noun>] -transitive>] -transitive>")]
		public void Queries(string query)
		{
			RantProgram.CompileString(query);
		}
	}
}