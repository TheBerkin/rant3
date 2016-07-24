using System.Collections.Generic;

using Rant.Internals.Engine.Utilities;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard
{
	internal class RichList : RichActionBase
	{
        public List<RichActionBase> Items
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
        public List<RichActionBase> InternalItems { get { return _items; } set { _items = value; } }
        public bool ItemsChanged = false;

        private List<RichActionBase> _items;
        private List<RichActionBase> _realItems;
		private bool _concatSyntax = true;

		public RichList(Stringe origin, List<RichActionBase> items, bool concatSyntax = true)
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
				List<RichActionBase> tempItems = new List<RichActionBase>();
				for (var i = 0; i < _items.Count; i++)
				{
					var item = _items[i];
					if (item is RichActionBase)
					{
						yield return item;
						var val = sb.ScriptObjectStack.Pop();
						if (val is RichList && _concatSyntax)
							tempItems.AddRange((val as RichList).Items);
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
