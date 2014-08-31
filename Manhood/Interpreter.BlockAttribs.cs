using Manhood.Compiler;

namespace Manhood
{
    internal partial class Interpreter
    {
        public class BlockAttribs
        {
            public int Repetitons { get; set; }
            public Source Separator { get; set; }
            public Source Before { get; set; }
            public Source After { get; set; }

            public BlockAttribs()
            {
                Repetitons = 1;
                Separator = null;
                Before = null;
                After = null;
            }
        }
    }
}