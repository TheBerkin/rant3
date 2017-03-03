using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant.Vocabulary
{
	public class ClassTree
	{
		private const int MAX_DEPTH = 4;

		public ClassTreeNode RootNode;

		public ClassTree(IEnumerable<RantDictionaryEntry> entries)
		{
			RootNode = new ClassTreeNode()
			{
				Name = "root",
				Classes = new string[] { },
				Entries = entries.Where(e => !e.HasClasses).ToList()
			};

			var rootClasses = OrderClasses(new string[] { }, entries);
			foreach (var className in rootClasses)
			{
				var node = new ClassTreeNode() { Name = className, Classes = new string[] { className } };

				PopulateNode(node, entries.Where(e => e.ContainsClass(className)));
				CountNode(node);

				RootNode.ChildNodes[className] = node;
			}
		}

		public IEnumerable<RantDictionaryEntry> Query(IEnumerable<string> classes)
		{
			return QueryNode(RootNode, new HashSet<string>(classes));
		}

		private IEnumerable<RantDictionaryEntry> QueryNode(ClassTreeNode node, HashSet<string> classes)
		{
			if (node.DepthLimit)
			{
				return node.Entries.Where(e => classes.All(c => e.ContainsClass(c)));
			}

			// find the largest class
			var largestClass = RootNode.ChildNodes
				.Where(kv => classes.Contains(kv.Key))
				.OrderByDescending(kv => kv.Value.Count)
				.Select(kv => kv.Value)
				.FirstOrDefault();

			if (largestClass == default(ClassTreeNode))
			{
				return node.Entries;
			}

			return QueryNode(largestClass, new HashSet<string>(classes.Where(c => c != largestClass.Name)));
		}

		private void PopulateNode(ClassTreeNode node, IEnumerable<RantDictionaryEntry> entries)
		{
			node.Entries = entries.Where(e => e.ClassCount >= node.Classes.Length).ToList();
			if (node.Entries.Count == entries.Count()) return;

			var otherEntries = entries.Where(e => e.ClassCount > node.Classes.Length);

			if (node.Depth == MAX_DEPTH)
			{
				node.DepthLimit = true;
				return;
			}

			var classes = OrderClasses(node.Classes, otherEntries);
			foreach (var className in classes)
			{
				var childNode = new ClassTreeNode() { Name = className, Classes = node.Classes.Concat(new string[] { className }).ToArray() };
				childNode.Depth = node.Depth + 1;

				PopulateNode(childNode, otherEntries.Where(e => e.ContainsClass(className)));
				CountNode(node);

				node.ChildNodes[className] = childNode;
			}
		}

		private int CountNode(ClassTreeNode node)
		{
			var count = node.Entries.Count;

			foreach (var child in node.ChildNodes.Values)
			{
				count += CountNode(child);
			}

			node.Count = count;
			return count;
		}

		private IEnumerable<string> OrderClasses(string[] ignoreClasses, IEnumerable<RantDictionaryEntry> entries)
		{
			var classCounts = new Dictionary<string, int>();

			foreach (var entry in entries)
			{
				foreach (var c in entry.GetClasses())
				{
					if (ignoreClasses.Contains(c)) continue;

					classCounts[c] = (classCounts.ContainsKey(c) ? classCounts[c] + 1 : 1);
				}
			}

			return classCounts.OrderByDescending(kv => kv.Value).Select(kv => kv.Key);
		}
	}

	public class ClassTreeNode
	{
		public string Name;
		public string[] Classes;
		public int Count = 0;
		public Dictionary<string, ClassTreeNode> ChildNodes = new Dictionary<string, ClassTreeNode>();
		public List<RantDictionaryEntry> Entries = new List<RantDictionaryEntry>();
		public int Depth = 1;
		public bool DepthLimit = false;
	}
}
