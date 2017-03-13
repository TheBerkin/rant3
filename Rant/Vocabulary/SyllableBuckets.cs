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

using Rant.Vocabulary.Querying;

namespace Rant.Vocabulary
{
    internal class SyllableBuckets
    {
        private readonly Dictionary<int, List<RantDictionaryEntry>> _buckets = new Dictionary<int, List<RantDictionaryEntry>>();

        public SyllableBuckets(int termIndex, IEnumerable<RantDictionaryEntry> entries)
        {
            foreach (var entry in entries)
            {
                int syllableCount = entry[termIndex].SyllableCount;
                if (syllableCount == 0) continue;

                if (!_buckets.ContainsKey(syllableCount))
                    _buckets[syllableCount] = new List<RantDictionaryEntry>();

                _buckets[syllableCount].Add(entry);
            }
        }

        public List<RantDictionaryEntry> Query(RangeFilter filter)
        {
            if (_buckets.Count == 0) return new List<RantDictionaryEntry>();

            int min = filter.Minimum == null ? 1 : (int)filter.Minimum;
            int max = filter.Maximum == null ? min : (int)filter.Maximum;

            // go through twice, first to find out the size to allocate
            int size = 0;
            for (int i = min; i <= max; i++)
                size += _buckets[i].Count;

            var list = new List<RantDictionaryEntry>(size);
            for (int i = min; i <= max; i++)
                list.AddRange(_buckets[i]);

            return list;
        }
    }
}