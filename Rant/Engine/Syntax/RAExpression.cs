using System;
using System.Linq;
using System.Collections.Generic;
using Rant.Stringes;
using Rant.Engine.ObjectModel;
using Rant.Engine.Syntax.Richard;

namespace Rant.Engine.Syntax
{
	internal class RAExpression : RantAction
	{
		public readonly REAGroup Group;

		public RAExpression(IEnumerable<RantExpressionAction> actions, Stringe token, string sourceName)
			: base(token)
		{
			Group = new REAGroup(actions, token, sourceName);
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			foreach (RantExpressionAction action in Group.Actions)
				yield return action;
            if (sb.ScriptObjectStack.Any())
            {
                var obj = sb.ScriptObjectStack.Pop();
                if (obj is REAList)
                {
                    var list = (obj as REAList);
                    // don't make a block from a list with zero items
                    if (list.Items.Count == 0)
                        yield break;
                    List<RantAction> actions = new List<RantAction>();
                    foreach (RantExpressionAction action in list.Items)
                    {
                        yield return action;
                        var item = sb.ScriptObjectStack.Pop();
                        if (item is REAPatternString)
                            actions.Add((item as REAPatternString).Pattern.Action);
                        else
                            actions.Add(new RAText(action.Range, item.ToString()));
                    }
                    yield return new RABlock(list.Range, actions.ToArray());
                }
                else if (obj is REAPatternString)
                    yield return (obj as REAPatternString).Pattern.Action;
                else if (obj is bool)
                    sb.Print((bool)obj ? "true" : "false");
                else if(!(obj is RantObject && (obj as RantObject).Type == RantObjectType.Undefined))
                    sb.Print(obj);
            }
			yield break;
		}
	}
}
