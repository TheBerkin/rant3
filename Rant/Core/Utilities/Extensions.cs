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

namespace Rant.Core.Utilities
{
	internal static class Extensions
	{
		public static ulong RotL(this ulong data, int times)
		{
			return (data << (times % 64)) | (data >> (64 - times % 64));
		}

		public static ulong RotR(this ulong data, int times)
		{
			return (data >> (times % 64)) | (data << (64 - times % 64));
		}

		public static long Hash(this string input)
		{
			unchecked
			{
				long seed = 13;
				foreach (char c in input)
				{
					seed += c * 19;
					seed *= 6364136223846793005;
				}
				return seed;
			}
		}

		public static void AddRange<T>(this HashSet<T> hashset, params T[] items)
		{
			foreach (var item in items) hashset.Add(item);
		}

		public static void AddRange<T>(this HashSet<T> hashset, IEnumerable<T> items)
		{
			foreach (var item in items) hashset.Add(item);
		}
	}
}