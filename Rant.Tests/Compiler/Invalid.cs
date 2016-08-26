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
			RantProgram.CompileString(@"\");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedBlock()
		{
			RantProgram.CompileString(@"{");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnexpectedBlockTerminator()
		{
			RantProgram.CompileString(@"}");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void EmptyTag()
		{
			RantProgram.CompileString(@"[]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnexpectedTagTerminator()
		{
			RantProgram.CompileString(@"]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnexpectedQueryTerminator()
		{
			RantProgram.CompileString(@">");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedQuery()
		{
			RantProgram.CompileString(@"<noun");
		}

		[TestCase("\"")]
		[TestCase(".")]
		[TestCase("")]
		[TestCase("?")]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidQueryTableName(string name)
		{
			RantProgram.CompileString($"<{name}>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidQuerySubtype()
		{
			RantProgram.CompileString(@"<noun.???>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void MissingQuerySubtype()
		{
			RantProgram.CompileString(@"<noun.>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void MissingQueryClassFilter()
		{
			RantProgram.CompileString(@"<noun->");
		}

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void TooManyCarrierOperators()
	    {
	        RantProgram.CompileString(@"<noun::!A::@B>");
	    }

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void MissingQuantifierComma()
	    {
	        RantProgram.CompileString(@"\123a");
	    }

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void UnterminatedConstantLiteral()
	    {
	        RantProgram.CompileString("\"");
	    }

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void UnterminatedRegex()
	    {
	        RantProgram.CompileString("`");
	    }

	    [TestCase(".")]
        [TestCase(")")]
        [TestCase("FOO")]
        [TestCase("?!")]
        [TestCase("<noun>")]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void InvalidCarrierComponent(string carrier)
	    {
	        RantProgram.CompileString($"<noun::{carrier}>");
	    }

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void InvalidCarrierDelete()
		{
			RantProgram.CompileString(@"<::>");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void NonexistentFunction()
		{
			RantProgram.CompileString(@"[berkin_rules]");
		}

		[TestCase(@"[rep:way;too;many;arguments]")]		// too many
		[TestCase(@"[rep]")]							// too few
		[ExpectedException(typeof(RantCompilerException))]
		public void ParameterMismatch(string pattern)
		{
			RantProgram.CompileString(pattern);
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedFunctionCall()
		{
			RantProgram.CompileString(@"[rep:10");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void BadParameterNames()
		{
			RantProgram.CompileString(@"[$[epic_fail:good_param;bad=param]:NO]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void BadNameArgsSeparator()
		{
			RantProgram.CompileString(@"[numfmt;verbal-en]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void ExtraOpeningBracket()
		{
			RantProgram.CompileString(@"[[rep:10]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void MissingReplacerArgument()
		{
			RantProgram.CompileString(@"[`\s*`:this is a test]");
		}

		[Test]
		[ExpectedException(typeof(RantCompilerException))]
		public void UnterminatedQueryComplement()
		{
			RantProgram.CompileString(@"<verb.ing [the <noun>>");
		}
	}
}