using System.Collections.Generic;

using Rant.Core.Compiler.Syntax.Richard.Operators;
using Rant.Core.ObjectModel;
using Rant.Core.Stringes;
using Rant.Core.Utilities;

namespace Rant.Core.Compiler.Syntax.Richard
{
	internal class RichObjectPropertyAssignment : RichActionBase
	{
		public readonly object Name;

		private RichActionBase _object;
		private RichActionBase _value;
        public RichInfixOperator Operator;

		public RichObjectPropertyAssignment(Stringe origin, RichActionBase obj, RichActionBase value)
			: base(origin)
		{
			Name = origin.Value;
			_value = value;
			_object = obj;
		}

		public RichObjectPropertyAssignment(Stringe origin, RichActionBase nameExp, RichActionBase obj, RichActionBase value)
			: base(origin)
		{
			Name = nameExp;
			_value = value;
			_object = obj;
		}

		public override object GetValue(Sandbox sb)
		{
			return null;
		}
        
        private RantObject ConvertValue(object value)
        {
            if (Type == ActionValueType.Boolean)
                return new RantObject((bool)value);
            if (Type == ActionValueType.Number)
                return new RantObject((double)value);
            if (Type == ActionValueType.String)
                return new RantObject((string)value);
            return new RantObject(value);
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			string name = null;
			if (Name is string)
				name = Name as string;
			else if (Name is RichActionBase)
			{
				yield return (Name as RichActionBase);
				name = sb.ScriptObjectStack.Pop().ToString();
			}
			else
				yield break;
            // variable assignment
            if (_object == null)
            {
                object value = null;
                if (_value != null)
                {
                    var count = sb.ScriptObjectStack.Count;
                    yield return _value;
                    // no value was created, no value will be assigned
                    if (count >= sb.ScriptObjectStack.Count)
                        yield break;
                    value = sb.ScriptObjectStack.Pop();
                    if (Operator != null && sb.Objects[name] != null)
                    {
                        Operator.LeftSide = Util.ConvertToAction(Range, sb.Objects[name].Value);
                        Operator.RightSide = Util.ConvertToAction(Range, value);
                        yield return Operator;
                        value = sb.ScriptObjectStack.Pop();
                    }
                }
                sb.Objects[name] = ConvertValue(value);
                yield break;
            }
			yield return _object;
			var obj = sb.ScriptObjectStack.Pop();
            if (obj is RichObject)
            {
                RichActionBase value = _value;
                if (Operator != null && (obj as RichObject).Values[name] != null)
                {
                    Operator.LeftSide = (obj as RichObject).Values[name];
                    Operator.RightSide = _value;
                    yield return Operator;
                    value = Util.ConvertToAction(Range, sb.ScriptObjectStack.Pop());
                }
                (obj as RichObject).Values[name] = value;
            }
            else if (obj is RichList)
            {
                int index = -1;
                if (!int.TryParse(name, out index))
                    yield break;
                RichActionBase value = _value;
                yield return _value;
                value = Util.ConvertToAction(Range, sb.ScriptObjectStack.Pop());
                if (Operator != null && (obj as RichList).Items[index] != null)
                {
                    Operator.LeftSide = (obj as RichList).Items[index];
                    Operator.RightSide = _value;
                    yield return Operator;
                    value = Util.ConvertToAction(Range, sb.ScriptObjectStack.Pop());
                }
                (obj as RichList).ItemsChanged = true;
                if ((obj as RichList).InternalItems.Count <= index)
                    for (var i = (obj as RichList).InternalItems.Count - 1; i < index + 1; i++)
                        (obj as RichList).InternalItems.Add(null);
                (obj as RichList).InternalItems[index] = value;
                yield return (obj as RichList);
                obj = sb.ScriptObjectStack.Pop();
            }
			yield break;
		}
	}
}
