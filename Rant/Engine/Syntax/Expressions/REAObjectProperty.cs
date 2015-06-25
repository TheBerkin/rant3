using System;
using System.Collections.Generic;
using Rant.Stringes;
using Rant.Engine.ObjectModel;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REAObjectProperty : RantExpressionAction
	{
		public readonly object Name;
        public RantExpressionAction Object => _object;

		private RantExpressionAction _object;

		public REAObjectProperty(Stringe origin, RantExpressionAction obj)
			: base(origin)
		{
			Name = origin.Value;
			_object = obj;
		}

		public REAObjectProperty(Stringe origin, RantExpressionAction name, RantExpressionAction obj)
			: base(origin)
		{
			Name = name;
			_object = obj;
		}

		public override object GetValue(Sandbox sb)
		{
            return null;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			string name = null;
            if (Name is string)
                name = Name as string;
            else if (Name is RantExpressionAction)
            {
                yield return (Name as RantExpressionAction);
                name = sb.ScriptObjectStack.Pop().ToString();
            }
            else
                throw new RantRuntimeException(sb.Pattern, Range, "Invalid property name.");
            yield return _object;
            var obj = sb.ScriptObjectStack.Pop();
            if (RichardFunctions.HasProperty(obj.GetType(), name))
            {
                var enumerator = RichardFunctions.GetProperty(obj.GetType(), name).Invoke(sb, new object[] { new RantObject(obj) });
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
            else if (obj is REAList)
            {
                int index = -1;
                if (!int.TryParse(name, out index))
                    yield break;
                if (index > (obj as REAList).Items.Count - 1)
                    throw new RantRuntimeException(sb.Pattern, Range, "List access is out of bounds.");
                yield return (obj as REAList).Items[index];
            }
            else if (obj is REAObject)
            {
                var rObject = obj as REAObject;
                if (rObject.Values.ContainsKey(name))
                    yield return rObject.Values[name];
                else
                    sb.ScriptObjectStack.Push(new RantObject());
            }
            else if (obj is string)
            {
                int index = -1;
                if (!int.TryParse(name, out index))
                    yield break;
                if ((obj as string).Length <= index)
                    throw new RantRuntimeException(sb.Pattern, Range, "String character access is out of bounds.");
                sb.ScriptObjectStack.Push((obj as string)[index].ToString());
            }
            else
                sb.ScriptObjectStack.Push(new RantObject());
            if (obj == null || (obj is RantObject && (obj as RantObject).Value == null))
                throw new RantRuntimeException(sb.Pattern, Range, "Cannot access property of null object.");
            yield break;
		}
	}
}
