using Processus.Compiler;

namespace Processus.Arithmetic
{
    internal abstract class Expression
    {
        public abstract double Evaluate(Parser parser, Interpreter ii);
    }
}