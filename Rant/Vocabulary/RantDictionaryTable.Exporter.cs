using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rant.Vocabulary
{
    public partial class RantDictionaryTable
    {
        /// <summary>
        /// Saves the contents of the dictionary to a file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file to save.</param>
        public void Save(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("#version {0}", Version);
                writer.WriteLine("#name {0}", Name);
                writer.WriteLine("#subs {0}", Subtypes.Aggregate((c, n) => c + " " + n));
                writer.WriteLine();

                var entriesClean = Entries.Where(entry => !entry.NSFW);
                var entriesDirty = Entries.Where(entry => entry.NSFW);

                WriteEntries(writer, entriesClean);
                if (entriesDirty.Any())
                {
                    writer.WriteLine();
                    writer.WriteLine("#nsfw");
                    writer.WriteLine();
                    WriteEntries(writer, entriesDirty);
                }
            }
        }

        private static void WriteEntries(StreamWriter writer, IEnumerable<RantDictionaryEntry> entries)
        {
            foreach (var entry in entries)
            {
                writer.WriteLine("> {0}", entry.Terms.Select(t => t.Value).Aggregate((c, n) => c + "/" + n));

                if (!String.IsNullOrWhiteSpace(entry.Terms[0].Pronunciation))
                    writer.WriteLine("  | pron {0}", entry.Terms.Select(t => t.Pronunciation).Aggregate((c, n) => c + "/" + n));

                if (entry.Classes.Any())
                    writer.WriteLine("  | class {0}", entry.Classes.Aggregate((c, n) => c + " " + n));

                if (entry.Weight != 1)
                    writer.WriteLine("  | weight {0}", entry.Weight);
            }
        }
    }
}