using NUnit.Framework;

namespace Rant.Tests.Expressions
{
	public class Invalid
	{
		private readonly RantEngine rant = new RantEngine();

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void BlockAsVariable() => rant.Do("[@ x = { 2 } ]");

	    [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void IfStatementAsVariable() => rant.Do("[@ x = if(true) { 2 } ]");

	    [Test]
        [ExpectedException(typeof(RantRuntimeException))]
        public void WrongNumberOfArgs() => rant.Do("[@ x = function(a, b) { }; x(2) ]");

	    [Test]
        [ExpectedException(typeof(RantRuntimeException))]
        public void WrongNumberOfArgsNative() => rant.Do("[@ Output.print(2, 3) ]");

	    [Test]
        [ExpectedException(typeof(RantRuntimeException))]
        public void NumberAsFunction() => rant.Do("[@ (2)() ]");

	    [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void AWholeBunchOfNumbers() => rant.Do("[@ 1 2 2 2 3 3 4 5 6 6 ]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void MissingAdditionOperand() => rant.Do(@"[@ 0 + ]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void TooManyOperators() => rant.Do(@"[@ 1 + * / 2]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void TooManyOperatorsMissingOperand() => rant.Do(@"[@ 1 + * ]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void TooManyOperands() => rant.Do(@"[@ 1 + 1 1]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void TooManyOperatorsAndOperands() => rant.Do(@"[@ 1 + + 1 1]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void AssignmentToConstant() => rant.Do(@"[@ 1 = 2; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void AssignmentToString() => rant.Do(@"[@ ""foo"" = 2; ]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void TooManyEquals() => rant.Do(@"[@ var x = = 1; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void UnexpectedInfixAfterEquals() => rant.Do(@"[@ var x = * 1; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void UnexpectedInfixBeforeEquals() => rant.Do(@"[@ var x + = 1; ]");

        [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void AssignmentToKeyword() => rant.Do(@"[@ list = true; ]");
	}
}
