using System.Collections.Generic;
using System.Linq;

using Rant.Internals.Engine.Compiler.Syntax.Richard;
using Rant.Internals.Engine.ObjectModel;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax
{
	internal class RAExpression : RantAction
	{
		public readonly RichGroup Group;

		public RAExpression(IEnumerable<RichActionBase> actions, Stringe token, string sourceName)
			: base(token)
		{
			Group = new RichGroup(actions, token, sourceName);
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			foreach (RichActionBase action in Group.Actions)
				yield return action;
            if (sb.ScriptObjectStack.Any())
            {
                var obj = sb.ScriptObjectStack.Pop();
                if (obj is RichList)
                {
                    var list = (obj as RichList);
                    // don't make a block from a list with zero items
                    if (list.Items.Count == 0)
                        yield break;
                    List<RantAction> actions = new List<RantAction>();
                    foreach (RichActionBase action in list.Items)
                    {
                        yield return action;
                        var item = sb.ScriptObjectStack.Pop();
                        if (item is RichPatternString)
                            actions.Add((item as RichPatternString).Pattern.Action);
                        else
                            actions.Add(new RAText(action.Range, item.ToString()));
                    }
                    yield return new RABlock(list.Range, actions.ToArray());
                }
                else if (obj is RichPatternString)
                    yield return (obj as RichPatternString).Pattern.Action;
                else if (obj is bool)
                    sb.Print((bool)obj ? "true" : "false");
                else if(!(obj is RantObject && (obj as RantObject).Type == RantObjectType.Undefined))
                    sb.Print(obj);
            }
			yield break;
		}
	}
}
