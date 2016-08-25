using System;
using System.Text;

namespace Rant.Core.Compiler.Syntax
{
	internal sealed class RSTAttribute : Attribute
	{
		public uint TypeCode { get; }

		public RSTAttribute(string typeCodeString)
		{
			if (typeCodeString == null) throw new ArgumentNullException(nameof(typeCodeString));
			if (typeCodeString.Length != 4) throw new ArgumentException("Type code must be four characters long.");
			TypeCode = BitConverter.ToUInt32(Encoding.ASCII.GetBytes(typeCodeString), 0);
		}
	}
}