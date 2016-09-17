using System;

using Rant.Core.IO;

namespace Rant.Vocabulary.Querying
{
	internal abstract class Filter
	{
		internal const ushort FILTER_NONE = 0x0000;
		internal const ushort FILTER_CLASS = 0x0001;
		internal const ushort FILTER_REGEX = 0x0002;
		internal const ushort FILTER_SYLRANGE = 0x0003;
		internal const ushort FILTER_BIT_OTHER = 0x8000;

		public static Filter GetFilterInstance(ushort filterTypeCode)
		{
			switch (filterTypeCode)
			{
				case FILTER_NONE:
					return null;
				case FILTER_CLASS:
					return Activator.CreateInstance<ClassFilter>();
				case FILTER_SYLRANGE:
					return Activator.CreateInstance<RangeFilter>();
				case FILTER_REGEX:
					return Activator.CreateInstance<RegexFilter>();
				default:
					return null;
			}
		}

		/// <summary>
		/// Determines if the specified dictionary entry passes the filter.
		/// </summary>
		/// <param name="dictionary">The dictionary being queried.</param>
		/// <param name="table">The table being queried.</param>
		/// <param name="entry">The entry to test.</param>
		/// <param name="termIndex">The index of the term requested by the query.</param>
		/// <param name="query">The originating query.</param>
		/// <returns></returns>
		public abstract bool Test(RantDictionary dictionary, RantDictionaryTable table, RantDictionaryEntry entry, int termIndex, Query query);

		public abstract void Deserialize(EasyReader input);
		public abstract void Serialize(EasyWriter output);
	}
}