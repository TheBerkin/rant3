using System;

namespace Rant.Engine
{
	internal class RantParameter
	{
		public RantParameterType RantType { get; private set; }
		public Type NativeType { get; private set; }
		public string Name { get; private set; }
        public bool IsParams { get; private set; }

		public RantParameter(string name, Type nativeType, RantParameterType rantType, bool isParams = false)
		{
			Name = name;
			NativeType = nativeType;
			RantType = rantType;
		    IsParams = isParams;
		}
	}
}