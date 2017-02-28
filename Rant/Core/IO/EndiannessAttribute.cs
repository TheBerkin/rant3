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

namespace Rant.Core.IO
{
	/// <summary>
	/// Specifies the byte order in which a field should be written and read by EasyWriter/EasyReader.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	internal class EndiannessAttribute : Attribute
	{
		/// <summary>
		/// The endianness to represent the data in.
		/// </summary>
		public readonly Endian Endian;

		/// <summary>
		/// Initializes a new instance of the EasyIO.EndiannessAttribute class with the specified endianness.
		/// </summary>
		/// <param name="endianness">The endianness to represent the field data in.</param>
		public EndiannessAttribute(Endian endianness)
		{
			Endian = endianness;
		}
	}
}