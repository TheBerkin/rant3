using Rant.Engine.ObjectModel;
using Rant.Stringes;
using System;
using System.Collections.Generic;

namespace Rant.Engine.Syntax.Expressions
{
    internal class REANativeFunction : RantExpressionAction
    {
        private int _argCount;
        public RantObject That;
        private Func<RantObject, Sandbox, object[], IEnumerator<RantExpressionAction>> _function;

        public REANativeFunction(Stringe token, int argCount, Func<RantObject, Sandbox, object[], IEnumerator<RantExpressionAction>> function)
            : base(token)
        {
            _argCount = argCount;
            _function = function;
        }

        public override object GetValue(Sandbox sb)
        {
            return this;
        }

        public IEnumerator<RantExpressionAction> Execute(Sandbox sb)
        {
            List<object> args = new List<object>();
            for (var i = 0; i < _argCount; i++)
                args.Add(sb.ScriptObjectStack.Pop());
            args.Reverse();
            IEnumerator<RantExpressionAction> iterator = null;
            while (true)
            {
                try
                {
                    if(iterator == null)
                        iterator = _function.Invoke(That, sb, args.ToArray());
                    if (!iterator.MoveNext())
                        break;
                }
                // attach token to it and throw it up
                catch (RantRuntimeException e)
                {
                    e.SetToken(Range);
                    throw e;
                }
                yield return iterator.Current;
            }
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield break;
        }
    }
}
