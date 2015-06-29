using System.Collections.Generic;

using Rant.Engine.ObjectModel;
using Rant.Engine.Syntax.Richard.Operators;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAObjectPropertyAssignment : RantExpressionAction
	{
		public readonly object Name;

		private RantExpressionAction _object;
		private RantExpressionAction _value;
        public REAInfixOperator Operator;

		public REAObjectPropertyAssignment(Stringe origin, RantExpressionAction obj, RantExpressionAction value)
			: base(origin)
		{
			Name = origin.Value;
			_value = value;
			_object = obj;
		}

		public REAObjectPropertyAssignment(Stringe origin, RantExpressionAction nameExp, RantExpressionAction obj, RantExpressionAction value)
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
			else if (Name is RantExpressionAction)
			{
				yield return (Name as RantExpressionAction);
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
            if (obj is REAObject)
            {
                RantExpressionAction value = _value;
                if (Operator != null && (obj as REAObject).Values[name] != null)
                {
                    Operator.LeftSide = (obj as REAObject).Values[name];
                    Operator.RightSide = _value;
                    yield return Operator;
                    value = Util.ConvertToAction(Range, sb.ScriptObjectStack.Pop());
                }
                (obj as REAObject).Values[name] = value;
            }
            else if (obj is REAList)
            {
                int index = -1;
                if (!int.TryParse(name, out index))
                    yield break;
                RantExpressionAction value = _value;
                yield return _value;
                value = Util.ConvertToAction(Range, sb.ScriptObjectStack.Pop());
                if (Operator != null && (obj as REAList).Items[index] != null)
                {
                    Operator.LeftSide = (obj as REAList).Items[index];
                    Operator.RightSide = _value;
                    yield return Operator;
                    value = Util.ConvertToAction(Range, sb.ScriptObjectStack.Pop());
                }
                (obj as REAList).ItemsChanged = true;
                if ((obj as REAList).InternalItems.Count <= index)
                    for (var i = (obj as REAList).InternalItems.Count - 1; i < index + 1; i++)
                        (obj as REAList).InternalItems.Add(null);
                (obj as REAList).InternalItems[index] = value;
                yield return (obj as REAList);
                obj = sb.ScriptObjectStack.Pop();
            }
			yield break;
		}
	}
}
