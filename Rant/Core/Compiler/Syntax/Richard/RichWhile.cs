using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
    internal class RichWhile : RichActionBase
    {
        RichActionBase _test;
        RichActionBase _body;

        public RichWhile(Stringe token, RichActionBase test, RichActionBase body)
            : base(token)
        {
            _test = test;
            _body = body;

            Breakable = true;
        }

        public override object GetValue(Sandbox sb)
        {
            return null;
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            while (true)
            {
                yield return _test;
                var result = sb.ScriptObjectStack.Pop();
                if (!(result is bool))
                    throw new RantRuntimeException(sb.Pattern, Range, "Expected boolean value in while statement.");
                if (!(bool)result)
                    yield break;
                sb.Objects.EnterScope();
                yield return _body;
                sb.Objects.ExitScope();
            }
        }
    }
}
