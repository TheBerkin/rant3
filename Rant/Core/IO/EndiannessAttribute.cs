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
