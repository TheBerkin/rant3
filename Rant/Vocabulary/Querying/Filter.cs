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
		public abstract int Priority { get; }

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