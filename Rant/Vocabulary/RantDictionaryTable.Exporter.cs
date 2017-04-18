using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant.Core.Utilities;
using Rant.Vocabulary.Utilities;

namespace Rant.Vocabulary
{
	public partial class RantDictionaryTable
	{
		/// <summary>
		/// Saves the contents of the dictionary to a file at the specified path.
		/// </summary>
		/// <param name="path">The path to the file to save.</param>
		/// <param name="useDiffmark">Specifies whether to generate Diffmarked entries.</param>
		public void Save(string path, bool useDiffmark = false)
		{
			using (var writer = new StreamWriter(path))
			{
				writer.WriteLine("#name {0}", Name);
				writer.WriteLine("#subs {0}", Subtypes.Aggregate((c, n) => c + " " + n));
				foreach (string hiddenClass in _hidden)
					writer.WriteLine($"#hidden {hiddenClass}");
				// TODO: Export types for tables
				writer.WriteLine();

				WriteEntries(writer, _entriesHash, useDiffmark);
			}
		}

		private static void WriteEntries(StreamWriter writer, IEnumerable<RantDictionaryEntry> entries, bool diffmark = false)
		{
			var root = new RantDictionaryTableClassDirective("root");
			// we create the tree of class directives first - then we fill it
			var classes = entries.Where(x => GetClassesForExport(x).Any()).Select(x => GetClassesForExport(x).ToArray()).ToList();
			if (classes.Any())
				CreateNestedClassDirectives(root, classes);

			// now that we have a tree of class directives, let's populate it
			foreach (var entry in entries)
			{
				if (!GetClassesForExport(entry).Any())
				{
					root.Entries.Add(entry);
					continue;
				}
				root.FindDirectiveForClasses(GetClassesForExport(entry).ToArray())?.Entries.Add(entry);
			}

			root.Prune();

			root.Render(writer, -1, diffmark);
		}

		// thank you stack exchange
		// http://programmers.stackexchange.com/q/267495/161632
		private static void CreateNestedClassDirectives(RantDictionaryTableClassDirective parent, List<string[]> entries)
		{
			while (true)
			{
				var classCounts = new Dictionary<string, int>();

				foreach (var x in entries)
				{
					int count;
					for (int i = 0; i < x.Length; i++)
						classCounts[x[i]] = classCounts.TryGetValue(x[i], out count) ? count + 1 : 1;
				}

				// do this again with children of this class
				string bestClass = classCounts.OrderByDescending(x => x.Value).First().Key;
				if (classCounts[bestClass] <= 3) return;
				var bestDirective = new RantDictionaryTableClassDirective(bestClass);
				bestDirective.Parent = parent;
				var childEntries = entries.Where(x => x.Contains(bestClass) && x.Length > 1).Select(x =>
				{
					// remove bestClass from array
					return x.Where(y => y != bestClass).ToArray();
				}).ToList();
				if (childEntries.Any()) CreateNestedClassDirectives(bestDirective, childEntries);
				parent.Children[bestClass] = bestDirective;

				// for things that aren't children of this class
				var otherEntries = entries.Where(x => !x.Contains(bestClass) && x.Length > 0).ToList();
				if (otherEntries.Any())
				{
					entries = otherEntries;
					continue;
				}
				break;
			}
		}

		// Returns the classes of an object, but optional classes are postfixed with ?
		private static IEnumerable<string> GetClassesForExport(RantDictionaryEntry entry)
		{
			return entry.GetRequiredClasses().Concat(entry.GetOptionalClasses().Select(x => x + "?"));
		}

		private class RantDictionaryTableClassDirective
		{
			public readonly Dictionary<string, RantDictionaryTableClassDirective> Children;
			public readonly List<RantDictionaryEntry> Entries;
			public readonly string Name;
			public RantDictionaryTableClassDirective Parent;

			public RantDictionaryTableClassDirective(string name)
			{
				Name = name;
				Entries = new List<RantDictionaryEntry>();
				Children = new Dictionary<string, RantDictionaryTableClassDirective>();
			}

			public string[] Classes
			{
				get
				{
					if (Parent == null) return new string[0];
					var classes = new List<string> { Name };
					classes.AddRange(Parent.Classes);
					return classes.ToArray();
				}
			}

			public bool ShouldPrune
			{
				get { return Parent != null && Entries.Count <= 3 && !Children.Any(); }
			}

			public RantDictionaryTableClassDirective FindDirectiveForClasses(string[] classes)
			{
				foreach (string c in classes.Where(c => Children.ContainsKey(c)))
				{
					if (classes.Length == 1) return Children[c];
					var tree = Children[c].FindDirectiveForClasses(classes.Where(x => x != c).ToArray());
					if (tree != null)
						return tree;
				}
				if (!Children.Any()) return this;
				// all children were null, we're the best we've got I guess
				return this;
			}

			public void Prune()
			{
				if (ShouldPrune)
				{
					if (Parent == null) return;
					Parent.Entries.AddRange(Entries);
					Parent.Children.Remove(Name);
				}
				var children = Children.Values.ToList();
				foreach (var child in children)
					child.Prune();
			}

			public void Render(StreamWriter writer, int level = -1, bool diffmark = false)
			{
				string leadingWhitespace = "";
				string leadingWhitespacer = (Parent != null ? "  " : "");
				for (int i = 0; i < level; i++)
				{
					leadingWhitespacer += "  ";
					leadingWhitespace += "  ";
				}

				if (Parent != null)
					writer.WriteLine(leadingWhitespace + "#class add " + Name);

				foreach (string key in Children.Keys.OrderBy(x => x))
					Children[key].Render(writer, level + 1, diffmark);

				foreach (var entry in Entries.OrderBy(x => x[0].Value))
				{
					if (entry.TermCount > 1 && diffmark)
					{
						writer.WriteLine(leadingWhitespacer + ">> {0}",
							entry.GetTerms()
								.Select((t, i) => i == 0 ? t.Value : Diff.Derive(entry[0].Value, t.Value))
								.Aggregate((c, n) => c + "/" + n));
					}
					else
					{
						writer.WriteLine(leadingWhitespacer + "> {0}",
							entry.GetTerms().Select(t => t.Value).Aggregate((c, n) => c + "/" + n));
					}

					if (!Util.IsNullOrWhiteSpace(entry[0].Pronunciation))
						writer.WriteLine(leadingWhitespacer + "  | pron {0}",
							entry.GetTerms().Select(t => t.Pronunciation).Aggregate((c, n) => c + "/" + n));

					var uniqueClasses = GetClassesForExport(entry).Where(x => !Classes.Contains(x)).OrderBy(x => x).ToArray();
					if (uniqueClasses.Length > 0)
						writer.WriteLine(leadingWhitespacer + "  | class {0}", uniqueClasses.Aggregate((c, n) => c + " " + n));

					if (entry.Weight != 1)
						writer.WriteLine(leadingWhitespacer + "  | weight {0}", entry.Weight);
				}

				if (Parent != null)
					writer.WriteLine(leadingWhitespace + "#class remove " + Name + "\n");
			}
		}
	}
}