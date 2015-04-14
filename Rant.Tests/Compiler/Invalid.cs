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
		public void UnexpectedQueryTerminator()
		{
			RantPattern.FromString(@">");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedQuery()
		{
			RantPattern.FromString(@"<noun");
		}

		[TestCase("\"")]
		[TestCase(".")]
		[TestCase("")]
		[TestCase("?")]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidQueryTableName(string name)
		{
			RantPattern.FromString($"<{name}>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidQuerySubtype()
		{
			RantPattern.FromString(@"<noun.???>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void MissingQuerySubtype()
		{
			RantPattern.FromString(@"<noun.>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void MissingQueryClassFilter()
		{
			RantPattern.FromString(@"<noun->");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidCarrierDelete()
		{
			RantPattern.FromString(@"<::>");
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

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void BadParameterNames()
		{
			RantPattern.FromString(@"[$[epic_fail:good_param;bad=param]:NO]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void BadNameArgsSeparator()
		{
			RantPattern.FromString(@"[numfmt;verbal-en]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void ExtraOpeningBracket()
		{
			RantPattern.FromString(@"[[rep:10]");
		}
	}
}