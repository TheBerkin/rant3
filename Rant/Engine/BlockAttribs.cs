using System.Collections.Generic;

using Rant.Engine.Compiler;
using Rant.Stringes.Tokens;

namespace Rant.Engine
{
    internal class BlockAttribs
    {
        public int Repetitons { get; set; }
        public IEnumerable<Token<R>> Separator { get; set; }
        public IEnumerable<Token<R>> Before { get; set; }
        public IEnumerable<Token<R>> After { get; set; }
        public Synchronizer Sync { get; set; }

        public BlockAttribs()
        {
            Repetitons = 1;
            Separator = null;
            Before = null;
            After = null;
            Sync = null;
        }
    }
}