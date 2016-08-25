namespace Rant.Core.Compiler.Syntax
{
	internal sealed class DeserializeRequest
	{
		public DeserializeRequest(uint typeCode)
		{
			TypeCode = typeCode;
		}

		public uint TypeCode { get; }

		public RST Result { get; private set; }

		public void SetResult(RST rst) => Result = rst;
	}
}