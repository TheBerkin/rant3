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

using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Vocabulary.Querying;

namespace Rant.Vocabulary
{
    internal sealed class ClassCache
    {
        private readonly Dictionary<string, HashSet<RantDictionaryEntry>> _cache = new Dictionary<string, HashSet<RantDictionaryEntry>>();
        private readonly Dictionary<string, HashSet<RantDictionaryEntry>> _invCache = new Dictionary<string, HashSet<RantDictionaryEntry>>();

        public void BuildCache(RantDictionaryTable table)
        {
            _cache.Clear();
            _invCache.Clear();
            var clSet = new HashSet<string>();

            foreach (var entry in table.GetEntries())
            {
                foreach (var cl in entry.GetClasses())
                {
                    HashSet<RantDictionaryEntry> set;
                    clSet.Add(cl);
                    if (!_cache.TryGetValue(cl, out set))
                    {
                        set = _cache[cl] = new HashSet<RantDictionaryEntry>();
                    }
                    set.Add(entry);
                }
            }

            foreach (var cl in clSet)
            {
                if (!_invCache.TryGetValue(cl, out HashSet<RantDictionaryEntry> set))
                {
                    set = _invCache[cl] = new HashSet<RantDictionaryEntry>();
                }
                foreach (var entry in table.GetEntries())
                {
                    if (!entry.ContainsClass(cl)) set.Add(entry);
                }
            }
        }
        
        public IEnumerable<RantDictionaryEntry> Filter(IEnumerable<ClassFilterRule> rules, RantDictionary dictionary, RantDictionaryTable table)
        {
            var r = rules.ToArray();
            if (r.Length == 0) return table.GetEntries();
            HashSet<RantDictionaryEntry> setCached;
            var set = new HashSet<RantDictionaryEntry>();
            var hide = table.HiddenClasses
                // Exclude overridden hidden classes
                .Except(dictionary.IncludedHiddenClasses)
                // Exclude hidden classes filtered for
                .Where(cl => !r.Any(rule => rule.ShouldMatch && String.Equals(rule.Class, cl, StringComparison.InvariantCultureIgnoreCase))).ToArray();

            // Get initial pool
            if (r[0].ShouldMatch)
            {
                if (!_cache.TryGetValue(r[0].Class, out setCached)) return null;
            }
            else
            {
                if (!_invCache.TryGetValue(r[0].Class, out setCached)) setCached = table.EntriesHash;
            }

            foreach (var item in setCached)
            {
                if (hide.Length == 0 || !hide.Any(cl => item.ContainsClass(cl))) set.Add(item);
            }

            for (int i = 1; i < r.Length; i++)
            {
                set.IntersectWith(r[i].ShouldMatch ? _cache[r[i].Class] : _invCache[r[i].Class]);
            }

            return set;
        }
    }
}