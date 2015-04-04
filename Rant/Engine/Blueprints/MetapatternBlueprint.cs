namespace Rant.Engine.Blueprints
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
            var src = new RantPattern($"Meta_{srcstr.Hash():X16}", RantPatternSource.Metapattern, srcstr);
            I.PushState(new RantState(I, src, I.CurrentState.Output));
            return true;
        }
    }
}