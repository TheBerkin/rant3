using System;

namespace Rant.Engine.Constructs
{
    internal class Comparison
    {
        private readonly ComparisonResult _result;
        private readonly string _a;
        private readonly string _b;

        public ComparisonResult Result => _result;

        public string A => _a;

        public string B => _b;

        public Comparison(string a, string b)
        {
            _a = a;
            _b = b;
            double na, nb;
            if (!Double.TryParse(a, out na)) na = Double.NaN;
            if (!Double.TryParse(b, out nb)) nb = Double.NaN;
            bool ba = Util.BooleanRep(a);
            bool bb = Util.BooleanRep(b);

            _result |= a == b ? ComparisonResult.Equal : ComparisonResult.NotEqual;

            _result |= ba && bb ? ComparisonResult.All : ba || bb ? ComparisonResult.Any : ComparisonResult.None;
            if (ba != bb) _result |= ComparisonResult.One;
            if (na < nb) _result |= ComparisonResult.Less;
            if (na > nb) _result |= ComparisonResult.Greater;
        }
    }

    [Flags]
    internal enum ComparisonResult
    {
        NotEqual =  0x01,
        Different = NotEqual,
        Equal =     0x02,
        Less =      0x04,
        Greater =   0x08,
        All =       0x10,
        Any =       0x20,
        One =       0x40,
        None =      0x80
    }
}