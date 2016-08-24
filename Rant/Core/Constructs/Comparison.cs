using System;

using Rant.Core.Utilities;

namespace Rant.Core.Constructs
{
	internal class Comparison
	{
		public Comparison(string a, string b)
		{
			A = a;
			B = b;
			double na, nb;
			if (!double.TryParse(a, out na)) na = double.NaN;
			if (!double.TryParse(b, out nb)) nb = double.NaN;
			bool ba = Util.BooleanRep(a);
			bool bb = Util.BooleanRep(b);

			Result |= a == b ? ComparisonResult.Equal : ComparisonResult.NotEqual;

			Result |= ba && bb ? ComparisonResult.All : ba || bb ? ComparisonResult.Any : ComparisonResult.None;
			if (ba != bb) Result |= ComparisonResult.One;
			if (na < nb) Result |= ComparisonResult.Less;
			if (na > nb) Result |= ComparisonResult.Greater;
		}

		public ComparisonResult Result { get; }
		public string A { get; }
		public string B { get; }
	}

	[Flags]
	internal enum ComparisonResult
	{
		NotEqual = 0x01,
		Different = NotEqual,
		Equal = 0x02,
		Less = 0x04,
		Greater = 0x08,
		All = 0x10,
		Any = 0x20,
		One = 0x40,
		None = 0x80
	}
}