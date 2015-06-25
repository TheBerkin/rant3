using System;
using System.Collections.Generic;
using System.Linq;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions
{
    internal class REAFor : RantExpressionAction
    {
        RantExpressionAction _body;
        RantExpressionAction _expr;
        string _indexName;

        public REAFor(
            Stringe token,
            string indexName,
            RantExpressionAction body,
            RantExpressionAction expr)
            : base(token)
        {
            _expr = expr;
            _body = body;
            _indexName = indexName;
        }

        public override object GetValue(Sandbox sb)
        {
            return null;
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield return _expr;
            var expr = sb.ScriptObjectStack.Pop();
            if (!(expr is REAList) && !(expr is REAObject))
                throw new RantRuntimeException(sb.Pattern, Range, "Provided expression is not a list or object.");
            if (expr is REAList)
            {
                var items = (expr as REAList).Items;
                for (var i = 0; i < items.Count; i++)
                {
                    sb.Objects.EnterScope();
                    sb.Objects[_indexName] = new ObjectModel.RantObject(i);
                    yield return _body;
                    sb.Objects.RemoveLocal(_indexName);
                    sb.Objects.ExitScope();
                }
                yield break;
            }
            else if (expr is REAObject)
            {
                var items = (expr as REAObject).Values.Keys.ToList();
                foreach (string key in items)
                {
                    sb.Objects.EnterScope();
                    sb.Objects[_indexName] = new ObjectModel.RantObject(key);
                    yield return _body;
                    sb.Objects.RemoveLocal(_indexName);
                    sb.Objects.ExitScope();
                }
                yield break;
            }
        }
    }
}
