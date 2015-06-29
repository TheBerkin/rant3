using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAList : RantExpressionAction
	{
        public List<RantExpressionAction> Items
        {
            get
            {
                return _realItems;
            }
            set
            {
                _realItems = value;
                ItemsChanged = true;
            }
        }

		public override int GetHashCode() => Items.GetHashCode();
        public List<RantExpressionAction> InternalItems { get { return _items; } set { _items = value; } }
        public bool ItemsChanged = false;

        private List<RantExpressionAction> _items;
        private List<RantExpressionAction> _realItems;
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
			if (Items == null || ItemsChanged)
			{
				List<RantExpressionAction> tempItems = new List<RantExpressionAction>();
				for (var i = 0; i < _items.Count; i++)
				{
					var item = _items[i];
					if (item is RantExpressionAction)
					{
						yield return item;
						var val = sb.ScriptObjectStack.Pop();
						if (val is REAList && _concatSyntax)
							tempItems.AddRange((val as REAList).Items);
						else
							tempItems.Add(Util.ConvertToAction(item.Range, val));
					}
					else
						tempItems.Add(item);
                }
                Items = tempItems;
                ItemsChanged = false;
			}
			yield break;
		}

		public override string ToString()
		{
			return "list";
		}
	}
}
