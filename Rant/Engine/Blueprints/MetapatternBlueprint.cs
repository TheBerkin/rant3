using System;

namespace Rant.Blueprints
{
    internal sealed class MetapatternBlueprint : Blueprint
    {
        public MetapatternBlueprint(VM interpreter)
            : base(interpreter)
        {
        }

        public override bool Use()
        {
            var srcstr = I.PopResultString();
            var src = new RantPattern("Meta_\{srcstr.Hash():X16}", RantPatternSource.Metapattern, srcstr);
            I.PushState(new VM.State(I, src, I.CurrentState.Output));
            return true;
        }
    }
}