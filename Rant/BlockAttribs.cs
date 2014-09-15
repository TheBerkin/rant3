using System.Collections.Generic;

using Rant.Compiler;

using Stringes.Tokens;

namespace Rant
{
    internal class BlockAttribs
    {
        public int Repetitons { get; set; }
        public IEnumerable<Token<TokenType>> Separator { get; set; }
        public IEnumerable<Token<TokenType>> Before { get; set; }
        public IEnumerable<Token<TokenType>> After { get; set; }
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