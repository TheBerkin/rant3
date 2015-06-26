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
        public void EmptyGroupAsFunction() => rant.Do("[@ ()() ]");

	    public void UnexpectedInfixAfterEquals() => rant.Do(@"[@ var x = * 1; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void UnexpectedInfixBeforeEquals() => rant.Do(@"[@ var x + = 1; ]");

        [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void AssignmentToKeyword() => rant.Do(@"[@ list = true; ]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void NakedIncrement() => rant.Do(@"[@ ++ ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void NakedDecrement() => rant.Do(@"[@ -- ]");

        [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void PostIncrementConstant() => rant.Do(@"[@ 1++; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void PreIncrementConstant() => rant.Do(@"[@ ++1; ]");

	    [Test]
	    [ExpectedException(typeof(RantCompilerException))]
	    public void NumberAsFunctionCall() => rant.Do(@"[@ 1(); ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void OperatorAsFunctionCall() => rant.Do(@"[@ *(); ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void EmptyParentheses() => rant.Do(@"[@ () ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void IncrementNo() => rant.Do(@"[@ no++ ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void IncrementUndefined() => rant.Do(@"[@ ???++ ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void InvokeUndefined() => rant.Do(@"[@ ???(); ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void UndefinedUndefined() => rant.Do(@"[@ ??? ??? ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void InvokeNo() => rant.Do(@"[@ no(); ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void InvokeString() => rant.Do(@"[@ ""foobar""(); ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void InvokeList() => rant.Do(@"[@ (1, 2, 3)(); ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void InvokeObject() => rant.Do(@"[@ { x:1, y:2, z:3 }(); ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void AssignToUndefined() => rant.Do(@"[@ ??? = 100; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void AssignToFunctionCall() => rant.Do(@"[@ var func = function() {  }; func() = no; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void AssignToLambda() => rant.Do(@"[@ (() => Output.print(""test"")) = 100; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void IncrementLambda() => rant.Do(@"[@ (() => Output.print(""test""))++; ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void LambdaLambda() => rant.Do(@"[@ (() => Output.print(""foo"")) (() => Output.print(""bar"")) ]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void UndefinedAsObjectKey() => rant.Do(@"[@{ var invalid = { ???: 123 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void MissingObjectKey() => rant.Do(@"[@{ var invalid = { : 123 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectKeyExtraColon() => rant.Do(@"[@{ var invalid = { foo:: 123 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectKeyMissingColon() => rant.Do(@"[@{ var invalid = { foo 123 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectKeyMissingValue() => rant.Do(@"[@{ var invalid = { foo: }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectKeyAllAlone() => rant.Do(@"[@{ var invalid = { foo }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectTooManyValues() => rant.Do(@"[@{ var invalid = { foo: 123 456 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectMissingNeighboringKeyA() => rant.Do(@"[@{ var invalid = { foo: 123, 456, 789 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectMissingNeighboringKeyB() => rant.Do(@"[@{ var invalid = { foo: 123, 456, 789, bar: 123, 456, 789 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectNumericKey() => rant.Do(@"[@{ var invalid = { 123: 456 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectUnexpectedSemicolonA() => rant.Do(@"[@{ var invalid = { foo: 123; bar: 456, wow: 789 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectUnexpectedSemicolonB() => rant.Do(@"[@{ var invalid = { foo: 123, bar: 456; wow: 789 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectUnexpectedSemicolonC() => rant.Do(@"[@{ var invalid = { foo: 123, bar: 456, wow: 789; }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectNestedBraces() => rant.Do(@"[@{ var invalid = {{ foo: 123, bar: 456, wow: 789 }}; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void ObjectExtraEndBrace() => rant.Do(@"[@{ var invalid = { foo: 123, bar: 456, wow: 789 }}; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void DoubleObjectA() => rant.Do(@"[@{ var invalid = {}{}; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void DoubleObjectB() => rant.Do(@"[@{ var invalid = { foo: 123, bar: 456; wow: 789 }{}; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void DoubleObjectC() => rant.Do(@"[@{ var invalid = {}{ foo: 123, bar: 456; wow: 789 }; }]");

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void DoubleObjectD() => rant.Do(@"[@{ var invalid = { foo: 123, bar: 456; wow: 789 }{ foo: 123, bar: 456; wow: 789 }; }]");
    }
}
