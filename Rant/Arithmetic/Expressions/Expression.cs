using Rant.Compiler;

namespace Rant.Arithmetic
{
    internal abstract class Expression
    {
        public abstract double Evaluate(Parser parser, Interpreter ii);
    }
}