#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Rant.Vocabulary
{
    public class ClassTree
    {
        private const int MAX_DEPTH = 4;

        public ClassTreeNode RootNode;

        public ClassTree(IEnumerable<RantDictionaryEntry> entries)
        {
            RootNode = new ClassTreeNode
            {
                Name = "root",
                Classes = new string[] { },
                Entries = entries.Where(e => !e.HasClasses).ToList()
            };

            var rootClasses = OrderClasses(new string[] { }, entries);
            foreach (string className in rootClasses)
            {
                var node = new ClassTreeNode { Name = className, Classes = new[] { className } };

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
                return node.Entries.Where(e => classes.All(c => e.ContainsClass(c)));

            // find the largest class
            var largestClass = node.ChildNodes
                .Where(kv => classes.Contains(kv.Key))
                .OrderByDescending(kv => kv.Value.Count)
                .Select(kv => kv.Value)
                .FirstOrDefault();

            if (largestClass == default(ClassTreeNode))
            {
                if (classes.Any())
                    return new List<RantDictionaryEntry>();
                return node.Entries;
            }

            return QueryNode(largestClass, new HashSet<string>(classes.Where(c => c != largestClass.Name)));
        }

        private void PopulateNode(ClassTreeNode node, IEnumerable<RantDictionaryEntry> entries)
        {
            node.Entries = entries.Where(e => e.ClassCount >= node.Classes.Length).ToList();
            var otherEntries = entries.Where(e => e.ClassCount > node.Classes.Length);

            if (node.Depth == MAX_DEPTH)
            {
                node.DepthLimit = true;
                return;
            }

            var classes = OrderClasses(node.Classes, otherEntries);
            foreach (string className in classes)
            {
                var childNode = new ClassTreeNode { Name = className, Classes = node.Classes.Concat(new[] { className }).ToArray() };
                childNode.Depth = node.Depth + 1;

                PopulateNode(childNode, otherEntries.Where(e => e.ContainsClass(className)));
                CountNode(node);

                node.ChildNodes[className] = childNode;
            }
        }

        private int CountNode(ClassTreeNode node)
        {
            int count = node.Entries.Count;

            foreach (var child in node.ChildNodes.Values)
                count += CountNode(child);

            node.Count = count;
            return count;
        }

        private IEnumerable<string> OrderClasses(string[] ignoreClasses, IEnumerable<RantDictionaryEntry> entries)
        {
            var classCounts = new Dictionary<string, int>();

            foreach (var entry in entries)
            {
                foreach (string c in entry.GetClasses())
                {
                    if (ignoreClasses.Contains(c)) continue;

                    classCounts[c] = classCounts.ContainsKey(c) ? classCounts[c] + 1 : 1;
                }
            }

            return classCounts.OrderByDescending(kv => kv.Value).Select(kv => kv.Key);
        }
    }

    public class ClassTreeNode
    {
        public Dictionary<string, ClassTreeNode> ChildNodes = new Dictionary<string, ClassTreeNode>();
        public string[] Classes;
        public int Count = 0;
        public int Depth = 1;
        public bool DepthLimit = false;
        public List<RantDictionaryEntry> Entries = new List<RantDictionaryEntry>();
        public string Name;
    }
}