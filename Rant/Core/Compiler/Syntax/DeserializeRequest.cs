namespace Rant.Core.Compiler.Syntax
{
	internal sealed class DeserializeRequest
	{
		public DeserializeRequest()
		{
		}

		public RST Result { get; private set; }

		public void SetResult(RST rst) => Result = rst;
	}
}