using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Expressions
{
    internal class RantObjectExpression : RantExpression
    {
        private readonly Token<R> _token;
        private readonly RantObject _object;

        public RantObjectExpression(RantObject rantObj, Token<R> token)
        {
            _token = token;
            _object = rantObj;
        }

        public override RantObject Evaluate(VM vm)
        {
            if (_object == null)
                throw new RantException(vm.CurrentState.Reader.Source, _token, "What did you DO??!");
            return _object;
        }
    }
}