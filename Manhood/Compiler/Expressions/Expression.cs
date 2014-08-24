namespace Manhood.Compiler.Expressions
{
    internal abstract class Expression
    {
        public abstract string Evaluate(Interpreter ii);
    }
}