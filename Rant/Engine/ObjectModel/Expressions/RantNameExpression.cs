using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Expressions
{
    internal class RantNameExpression : RantExpression
    {
        private readonly Token<R> _token;

        public RantNameExpression(Token<R> token)
        {
            _token = token;
        }

        public override RantObject Evaluate(VM vm)
        {
            var obj = vm.Objects[_token.Value];
            if (obj == null)
                throw new RantException(vm.CurrentState.Reader.Source, _token, "Tried to access undefined variable '\{_token.Value}'.");
            return obj;
        }
    }
}