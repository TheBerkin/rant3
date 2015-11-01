using System.Collections.Generic;
using System.Linq;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
    internal class RichFor : RichActionBase
    {
        RichActionBase _body;
        RichActionBase _expr;
        string _indexName;

        public RichFor(
            Stringe token,
            string indexName,
            RichActionBase body,
            RichActionBase expr)
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
            if (!(expr is RichList) && !(expr is RichObject))
                throw new RantRuntimeException(sb.Pattern, Range, "Provided expression is not a list or object.");
            if (expr is RichList)
            {
                yield return expr as RichList;
                var list = sb.ScriptObjectStack.Pop() as RichList;
                var items = (list as RichList).Items;
                for (var i = 0; i < items.Count; i++)
                {
                    sb.Objects.EnterScope();
                    sb.Objects[_indexName] = new ObjectModel.RantObject(i);
                    yield return _body;
                    sb.Objects.ExitScope();
                }
                yield break;
            }
            else if (expr is RichObject)
            {
                var items = (expr as RichObject).Values.Keys.ToList();
                foreach (string key in items)
                {
                    sb.Objects.EnterScope();
                    sb.Objects[_indexName] = new ObjectModel.RantObject(key);
                    yield return _body;
                    sb.Objects.ExitScope();
                }
                yield break;
            }
        }
    }
}
