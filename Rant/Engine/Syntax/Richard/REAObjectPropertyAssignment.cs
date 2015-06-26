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

        private RantExpressionAction ConvertToAction(Stringe token, object value)
        {
            if (value is double)
                return new REANumber((double)value, token);
            if (value is string)
                return new REAString((string)value, token);
            if (value is bool)
                return new REABoolean(token, (bool)value);
            return value as RantExpressionAction;
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
                        Operator.LeftSide = ConvertToAction(Range, sb.Objects[name].Value);
                        Operator.RightSide = ConvertToAction(Range, value);
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
                    value = ConvertToAction(Range, sb.ScriptObjectStack.Pop());
                }
                (obj as REAObject).Values[name] = value;
            }
            else if (obj is REAList)
            {
                int index = -1;
                if (!int.TryParse(name, out index))
                    yield break;
                RantExpressionAction value = _value;
                if (Operator != null && (obj as REAList).Items[index] != null)
                {
                    Operator.LeftSide = (obj as REAList).Items[index];
                    Operator.RightSide = _value;
                    yield return Operator;
                    value = ConvertToAction(Range, sb.ScriptObjectStack.Pop());
                }
                (obj as REAList).Items[index] = value;
            }
			yield break;
		}
	}
}
