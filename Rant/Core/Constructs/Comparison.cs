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