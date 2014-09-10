using System;

using Manhood.Compiler;

namespace Manhood.Blueprints
{
    internal sealed class MetapatternBlueprint : Blueprint
    {
        public MetapatternBlueprint(Interpreter interpreter)
            : base(interpreter)
        {
        }

        public override bool Use()
        {
            var srcstr = I.PopResultString();
            var src = new Source("Meta_" + String.Format("{0:X16}", srcstr.Hash()), SourceType.Metapattern, srcstr);
            I.PushState(Interpreter.State.Create(src, I));
            return true;
        }
    }
}