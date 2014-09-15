using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant
{
    internal partial class Interpreter
    {
        private readonly Dictionary<string, Tuple<int, Channel>> _markers = new Dictionary<string, Tuple<int, Channel>>();

        public void SetMarker(string name)
        {
            var ch = CurrentState.Output.GetActive().First();
            _markers[name] = Tuple.Create(ch.Length, ch);
        }

        public int GetMarkerDistance(string a, string b)
        {
            Tuple<int, Channel> ma, mb;
            if (!_markers.TryGetValue(a, out ma) || !_markers.TryGetValue(b, out mb) || ma.Item2 != mb.Item2) return 0;
            return Math.Abs(mb.Item1 - ma.Item1);
        }
    }
}