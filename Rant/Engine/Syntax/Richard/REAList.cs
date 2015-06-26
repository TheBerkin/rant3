using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAList : RantExpressionAction
	{
        public List<RantExpressionAction> Items;

		public override int GetHashCode() => Items.GetHashCode();

		private List<RantExpressionAction> _items;
		private bool _concatSyntax = true;

		public REAList(Stringe origin, List<RantExpressionAction> items, bool concatSyntax = true)
			: base(origin)
		{
			_items = items;
			Type = ActionValueType.List;
			_concatSyntax = concatSyntax;
		}

		public override object GetValue(Sandbox sb)
		{
			return this;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			if (Items == null)
			{
				if (!_concatSyntax)
					Items = _items;
				else
				{
					List<RantExpressionAction> tempItems = new List<RantExpressionAction>();
					for (var i = 0; i < _items.Count; i++)
					{
						var item = _items[i];
						if (item is REAVariable)
						{
							yield return item;
							var val = sb.ScriptObjectStack.Pop();
							if (val is REAList)
								tempItems.AddRange((val as REAList).Items);
							else
								tempItems.Add(item);
						}
						else
							tempItems.Add(item);
					}
					Items = tempItems;
				}
			}
			yield break;
		}

		public override string ToString()
		{
			return "list";
		}
	}
}
