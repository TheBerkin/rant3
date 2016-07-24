using System.Collections.Generic;

using Rant.Core.Framework;
using Rant.Core.ObjectModel;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
	internal class RichObjectProperty : RichActionBase
	{
		public readonly object Name;
        public RichActionBase Object => _object;

		private RichActionBase _object;

		public RichObjectProperty(Stringe origin, RichActionBase obj)
			: base(origin)
		{
			Name = origin.Value;
			_object = obj;
		}

		public RichObjectProperty(Stringe origin, RichActionBase name, RichActionBase obj)
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
            else if (Name is RichActionBase)
            {
                var count = sb.ScriptObjectStack.Count;
                yield return (Name as RichActionBase);
                if (count >= sb.ScriptObjectStack.Count)
                    throw new RantRuntimeException(sb.Pattern, Range, "Expected value in bracket indexer.");
                name = sb.ScriptObjectStack.Pop().ToString();
            }
            else
                throw new RantRuntimeException(sb.Pattern, Range, "Invalid property name.");
            yield return _object;
            var obj = sb.ScriptObjectStack.Pop();
            if (RichardFunctions.HasProperty(obj.GetType(), name))
            {
                var prop = RichardFunctions.GetProperty(obj.GetType(), name);
                // bare property, like string.length
                if (!prop.TreatAsRichardFunction)
                {
                    var enumerator = prop.Invoke(sb, new object[] { new RantObject(obj) });
                    while (enumerator.MoveNext())
                        yield return enumerator.Current;
                }
                // function property, like list.push()
                else
                    // arg count is param count without the "that" property
                    yield return new RichNativeFunction(Range, prop.ParamCount - 1, prop) { That = new RantObject(obj) };
                yield break;
            }
            else if (obj is RichList)
            {
                int index = -1;
                if (!int.TryParse(name, out index))
                    yield break;
                yield return (obj as RichList);
                obj = sb.ScriptObjectStack.Pop();
                if (index > (obj as RichList).Items.Count - 1)
                    sb.ScriptObjectStack.Push(new RantObject(RantObjectType.Undefined));
                else
                    yield return (obj as RichList).Items[index];
                yield break;
            }
            else if (obj is RichObject)
            {
                var rObject = obj as RichObject;
                if (rObject.Values.ContainsKey(name))
                    yield return rObject.Values[name];
                yield break;
            }
            else if (obj is string)
            {
                int index = -1;
                if (!int.TryParse(name, out index))
                    yield break;
                if ((obj as string).Length <= index)
                    sb.ScriptObjectStack.Push(new RantObject(RantObjectType.Undefined));
                else
                    sb.ScriptObjectStack.Push((obj as string)[index].ToString());
                yield break;
            }
            if (obj == null || (obj is RantObject && (obj as RantObject).Value == null))
                throw new RantRuntimeException(sb.Pattern, Range, "Cannot access property of null object.");

            sb.ScriptObjectStack.Push(new RantObject(RantObjectType.Undefined));
            yield break;
		}
	}
}
