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
using System.Globalization;
using System.Linq;

namespace Rant.Core.Formatting
{
	internal class NumberFormatter
	{
		private static readonly NumberFormatInfo CommaGroupFormat = new NumberFormatInfo
		{
			NumberGroupSizes = new[] { 3 },
			NumberGroupSeparator = ",",
			NumberDecimalSeparator = "."
		};

		private static readonly NumberFormatInfo DotGroupFormat = new NumberFormatInfo
		{
			NumberGroupSizes = new[] { 3 },
			NumberGroupSeparator = ".",
			NumberDecimalSeparator = ","
		};

		public Endianness Endianness { get; set; } = Endianness.Default;
		public BinaryFormat BinaryFormat { get; set; } = BinaryFormat.Normal;
		public NumberFormat NumberFormat { get; set; } = NumberFormat.Normal;
		public int BinaryFormatDigits { get; set; } = 8;

		public string FormatNumber(double number)
		{
			switch (NumberFormat)
			{
				case NumberFormat.Normal:
					return number.ToString(CultureInfo.InvariantCulture);
				case NumberFormat.Group:
					return $"{number:n0}";
				case NumberFormat.GroupCommas:
					return number.ToString("n0", CommaGroupFormat);
				case NumberFormat.GroupDots:
					return number.ToString("n0", DotGroupFormat);
				case NumberFormat.Roman:
				case NumberFormat.RomanUpper:
					return Numerals.ToRoman(number);
				case NumberFormat.RomanLower:
					return Numerals.ToRoman(number, true);
				case NumberFormat.VerbalEn:
					return number % 1 > 0 ? "?" : Numerals.ToVerbal((long)number);
				case NumberFormat.Hex:
				case NumberFormat.HexUpper:
					return GetHex((long)number, true);
				case NumberFormat.HexLower:
					return GetHex((long)number, false);
				case NumberFormat.Binary:
					return GetBinary((long)number);
			}
			return number.ToString(CultureInfo.InvariantCulture);
		}

		private string GetHex(long number, bool uppercase)
		{
			string hexString = Convert.ToString(number, 16);
			int targetLength = BinaryFormat == BinaryFormat.Pad && hexString.Length < BinaryFormatDigits
				? BinaryFormatDigits
				: hexString.Length;
			int numBytes = targetLength % 2 != 0 ? (int)Math.Ceiling((double)targetLength / 2) : targetLength / 2;
			var bytes = new string[numBytes];
			if (BinaryFormat == BinaryFormat.Pad && hexString.Length < BinaryFormatDigits)
				for (int i = hexString.Length; i < BinaryFormatDigits; i++)
					hexString = '0' + hexString;
			if (hexString.Length % 2 != 0)
				hexString = "0" + hexString;

			for (int i = 0; i < bytes.Length; i++)
				bytes[i] = hexString[i * 2].ToString() + hexString[i * 2 + 1].ToString();

			if (Endianness != Endianness.Default && BitConverter.IsLittleEndian != (Endianness == Endianness.Little))
				bytes = bytes.Reverse().ToArray();

			string finalString = string.Join("", bytes);

			return uppercase ? finalString.ToUpper() : finalString;
		}

		private string GetBinary(long number)
		{
			bool needsReverse = Endianness != Endianness.Default &&
			                    BitConverter.IsLittleEndian != (Endianness == Endianness.Little);
			string hexString = Convert.ToString(number, 2);
			if (needsReverse && hexString.Length % 8 != 0) hexString = new string('0', hexString.Length % 8) + hexString;

			int origLength = hexString.Length;
			int finalLength =
				BinaryFormat == BinaryFormat.Pad
					? (origLength < BinaryFormatDigits * 4 ? BinaryFormatDigits * 4 : origLength)
					: BinaryFormat == BinaryFormat.Truncate
						? (origLength < BinaryFormatDigits * 4 ? origLength : BinaryFormatDigits * 4)
						: origLength;

			var chars = new char[finalLength];
			for (int i = 0; i < finalLength; i++) chars[i] = '0';

			if (needsReverse)
			{
				for (int i = 0; i < origLength; i += 8)
				for (int j = 0; j < 8; j++)
					chars[finalLength - i - (8 - j)] = hexString[i + j];
			}
			else
			{
				int truncatedOrigin = origLength > finalLength ? origLength - finalLength : 0;
				int truncatedLength = origLength > finalLength ? finalLength : origLength;
				hexString.CopyTo(truncatedOrigin, chars, finalLength - truncatedLength, truncatedLength);
			}
			return new string(chars);
		}
	}
}