using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Internals.Engine.Metadata;
using Rant.Internals.Engine.Utilities;

namespace Rant.Internals.Engine
{
	internal class RantParameter : IRantParameter
	{
		public RantParameterType RantType { get; private set; }
		public Type NativeType { get; private set; }
		public string Name { get; private set; }
        public bool IsParams { get; private set; }
        public string Description { get; set; }
	    public IEnumerable<IRantModeValue> GetEnumValues()
	    {
	        if (!NativeType.IsEnum) yield break;
	        foreach (var value in Enum.GetNames(NativeType))
	        {
                yield return new RantModeValue(Util.CamelToSnake(value), 
                    (NativeType.GetMember(value)[0].GetCustomAttributes(typeof(RantDescriptionAttribute), true).First() as RantDescriptionAttribute)?.Description ?? String.Empty);
	        }
	    }

	    public RantParameter(string name, Type nativeType, RantParameterType rantType, bool isParams = false)
		{
			Name = name;
			NativeType = nativeType;
			RantType = rantType;
		    IsParams = isParams;
            Description = String.Empty;
		}
	}
}