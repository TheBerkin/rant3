using System.Collections.Generic;
using System.Linq;

namespace Manhood
{
    internal class ListStore
    {
        private readonly Dictionary<string, List<string>> _lists;
        private readonly HashSet<string> _localLists;

        public ListStore()
        {
            _lists = new Dictionary<string, List<string>>();
            _localLists = new HashSet<string>();
        }

        public void CreateList(string name, ListType type)
        {
            if (!Util.ValidateName(name)) throw new ManhoodException("Invalid list name '"+name+"'.");

            if (type == ListType.Local) _localLists.Add(name);

            _lists[name] = new List<string>();
        }

        public void AddToList(Interpreter interpreter, string name, string value)
        {
            List<string> list;
            if (!_lists.TryGetValue(name, out list)) throw new ManhoodException("Tried to add to nonexistent list '"+name+"'.");

            list.Add(value);
        }

        public void ClearList(string name)
        {
            List<string> list;
            if (!_lists.TryGetValue(name, out list)) throw new ManhoodException("Tried to clear nonexistent list '" + name + "'.");

            list.Clear();
        }

        public void ReadLastItemFromList(Interpreter interpreter, string name, bool interpret, bool remove)
        {
            List<string> list;
            if (!_lists.TryGetValue(name, out list)) throw new ManhoodException("Tried to " + (remove ? "pop" : "peek") + " from nonexistent list '" + name + "'.");
            if (!list.Any()) throw new ManhoodException("Tried to " + (remove ? "pop" : "peek") + " from empty list '" + name + "'.");
            var item = list.Last();
            if (remove) list.RemoveAt(list.Count - 1);
            if (interpret)
            {
                interpreter.Do(item);
            }
            else
            {
                interpreter.Write(item);
            }
        }

        public void GetFromList(Interpreter interpreter, string name, int index, bool interpret)
        {
            List<string> list;
            if (!_lists.TryGetValue(name, out list)) throw new ManhoodException("Tried to get item from nonexistent list '"+name+"'.");
            if (index < 0 || index >= list.Count) throw new ManhoodException("Failed to get item from list '"+name+"': Index was out of range. (Item count = "+list.Count+", Index = "+index+")");
            if (interpret)
            {
                interpreter.Do(list[index]);
            }
            else
            {
                interpreter.Write(list[index]);
            }
        }

        public void RemoveFromList(string name, int index)
        {
            List<string> list;
            if (!_lists.TryGetValue(name, out list)) throw new ManhoodException("Tried to remove item from nonexistent list '" + name + "'.");
            if (index < 0 || index >= list.Count) throw new ManhoodException("Failed to remove item from list '" + name + "': Index was out of range. (Item count = " + list.Count + ", Index = " + index + ")");
            list.RemoveAt(index);
        }

        public void GetListItemCount(Interpreter interpreter, string name)
        {
            List<string> list;
            if (!_lists.TryGetValue(name, out list)) throw new ManhoodException("Tried to get count for nonexistent list '"+name+"'.");
            interpreter.Write(list.Count);
        }

        public void DoListAsBlock(Interpreter interpreter, string name)
        {
            List<string> list;
            if (!_lists.TryGetValue(name, out list)) throw new ManhoodException("Tried to block execute nonexistent list '"+name+"'.");
            interpreter.DoEnumerableAsBlock(list);
        }

        public void CopyList(string sourceList, string destList, ListCopyType copyType)
        {
            List<string> src, dest;
            if (!_lists.TryGetValue(sourceList, out src)) throw new ManhoodException("Tried to use nonexistent source list '"+sourceList+"' in copy operation.");
            if (!_lists.TryGetValue(destList, out dest)) throw new ManhoodException("Tried to use nonxistent destination list '"+destList+"' in copy operation.");

            switch (copyType)
            {
                case ListCopyType.Append:
                    dest.AddRange(src);
                    break;
                case ListCopyType.Prepend:
                    dest.InsertRange(0, src);
                    break;
            }
        }

        public void DestroyLocals()
        {
            foreach (var localName in _localLists)
            {
                _lists.Remove(localName);
            }
            _localLists.Clear();
        }
    }

    internal enum ListType
    {
        Global,
        Local
    }

    internal enum ListCopyType
    {
        Append,
        Prepend
    }
}