using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
    internal class REAWhile : RantExpressionAction
    {
        RantExpressionAction _test;
        RantExpressionAction _body;

        public REAWhile(Stringe token, RantExpressionAction test, RantExpressionAction body)
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
