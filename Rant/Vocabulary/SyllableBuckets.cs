using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
				var syllableCount = entry[termIndex].SyllableCount;
				if (syllableCount == 0) continue;

				if (!_buckets.ContainsKey(syllableCount))
				{
					_buckets[syllableCount] = new List<RantDictionaryEntry>();
				}

				_buckets[syllableCount].Add(entry);
			}
		}

		public List<RantDictionaryEntry> Query(RangeFilter filter)
		{
			if (_buckets.Count == 0) return new List<RantDictionaryEntry>();

			var min = (filter.Minimum == null ? 1 : (int)filter.Minimum);
			var max = (filter.Maximum == null ? min : (int)filter.Maximum);

			// go through twice, first to find out the size to allocate
			var size = 0;
			for (var i = min; i <= max; i++)
			{
				size += _buckets[i].Count;
			}

			var list = new List<RantDictionaryEntry>(size);
			for (var i = min; i <= max; i++)
			{
				list.AddRange(_buckets[i]);
			}

			return list;
		}
	}
}
