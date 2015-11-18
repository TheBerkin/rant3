namespace Rant.Engine.Output
{
	internal class OutputChainArticleBuffer : OutputChainBuffer
	{
		public OutputChainArticleBuffer(Sandbox sb, OutputChainBuffer prev) : base(sb, prev)
		{
		}

		public OutputChainArticleBuffer(Sandbox sb, OutputChainBuffer prev, OutputChainBuffer targetOrigin) : base(sb, prev, targetOrigin)
		{
		}
	}
}