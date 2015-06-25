using NUnit.Framework;

namespace Rant.Tests.Expressions
{
	public class Invalid
	{
		private readonly RantEngine rant = new RantEngine();

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void BlockAsVariable()
        {
            rant.Do("[@ x = { 2 } ]");
        }

        [Test]
        [ExpectedException(typeof(RantCompilerException))]
        public void IfStatementAsVariable()
        {
            rant.Do("[@ x = if(true) { 2 } ]");
        }

        [Test]
        [ExpectedException(typeof(RantRuntimeException))]
        public void WrongNumberOfArgs()
        {
            rant.Do("[@ x = function(a, b) { }; x(2) ]");
        }

        [Test]
        [ExpectedException(typeof(RantRuntimeException))]
        public void WrongNumberOfArgsNative()
        {
            rant.Do("[@ Output.print(2, 3) ]");
        }
	}
}
