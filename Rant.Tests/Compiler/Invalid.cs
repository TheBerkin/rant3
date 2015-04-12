using NUnit.Core;
using NUnit.Framework;

namespace Rant.Tests.Compiler
{
	[TestFixture]
	public class Invalid
	{
		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedBlock()
		{
			RantPattern.FromString(@"{");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnexpectedBlockTerminator()
		{
			RantPattern.FromString(@"}");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void EmptyTag()
		{
			RantPattern.FromString(@"[]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnexpectedTagTerminator()
		{
			RantPattern.FromString(@"]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void NonexistentFunction()
		{
			RantPattern.FromString(@"[berkin_rules]");
		}

		[TestCase(@"[rep:way;too;many;arguments]")]		// too many
		[TestCase(@"[rep]")]							// too few
		[ExpectedException(typeof(RantCompilerException))]
		public void ParameterMismatch(string pattern)
		{
			RantPattern.FromString(pattern);
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedFunctionCall()
		{
			RantPattern.FromString(@"[rep:10");
		}
	}
}