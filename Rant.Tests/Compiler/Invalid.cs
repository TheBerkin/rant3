using NUnit.Core;
using NUnit.Framework;

namespace Rant.Tests.Compiler
{
	[TestFixture]
	public class Invalid
	{
		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void IncompleteEscape()
		{
			RantPattern.CompileString(@"\");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedBlock()
		{
			RantPattern.CompileString(@"{");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnexpectedBlockTerminator()
		{
			RantPattern.CompileString(@"}");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void EmptyTag()
		{
			RantPattern.CompileString(@"[]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnexpectedTagTerminator()
		{
			RantPattern.CompileString(@"]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnexpectedQueryTerminator()
		{
			RantPattern.CompileString(@">");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedQuery()
		{
			RantPattern.CompileString(@"<noun");
		}

		[TestCase("\"")]
		[TestCase(".")]
		[TestCase("")]
		[TestCase("?")]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidQueryTableName(string name)
		{
			RantPattern.CompileString($"<{name}>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidQuerySubtype()
		{
			RantPattern.CompileString(@"<noun.???>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void MissingQuerySubtype()
		{
			RantPattern.CompileString(@"<noun.>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void MissingQueryClassFilter()
		{
			RantPattern.CompileString(@"<noun->");
		}

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void TooManyCarrierOperators()
	    {
	        RantPattern.CompileString(@"<noun::!A::@B>");
	    }

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void MissingQuantifierComma()
	    {
	        RantPattern.CompileString(@"\123a");
	    }

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void UnterminatedConstantLiteral()
	    {
	        RantPattern.CompileString("\"");
	    }

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void UnterminatedRegex()
	    {
	        RantPattern.CompileString("`");
	    }

	    [TestCase(".")]
        [TestCase(")")]
        [TestCase("FOO")]
        [TestCase("?!")]
        [TestCase("<noun>")]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void InvalidCarrierComponent(string carrier)
	    {
	        RantPattern.CompileString($"<noun::{carrier}>");
	    }

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidCarrierDelete()
		{
			RantPattern.CompileString(@"<::>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void NonexistentFunction()
		{
			RantPattern.CompileString(@"[berkin_rules]");
		}

		[TestCase(@"[rep:way;too;many;arguments]")]		// too many
		[TestCase(@"[rep]")]							// too few
		[ExpectedException(typeof(RantCompilerException))]
		public void ParameterMismatch(string pattern)
		{
			RantPattern.CompileString(pattern);
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedFunctionCall()
		{
			RantPattern.CompileString(@"[rep:10");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void BadParameterNames()
		{
			RantPattern.CompileString(@"[$[epic_fail:good_param;bad=param]:NO]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void BadNameArgsSeparator()
		{
			RantPattern.CompileString(@"[numfmt;verbal-en]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void ExtraOpeningBracket()
		{
			RantPattern.CompileString(@"[[rep:10]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void MissingReplacerArgument()
		{
			RantPattern.CompileString(@"[`\s*`:this is a test]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedQueryComplement()
		{
			RantPattern.CompileString(@"<verb.ing [the <noun>>");
		}
	}
}