using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Core.Utilities;
using Rant.Metadata;

namespace Rant.Core.Framework
{
	internal class RantFunctionParameter : IRantParameter
	{
		public RantFunctionParameter(string name, Type nativeType, RantFunctionParameterType rantType, bool isParams = false)
		{
			Name = name;
			NativeType = nativeType;
			RantType = rantType;
			IsParams = isParams;
			Description = string.Empty;
		}

		public Type NativeType { get; }
		public RantFunctionParameterType RantType { get; }
		public string Name { get; }
		public bool IsParams { get; }
		public string Description { get; set; }

		public IEnumerable<IRantModeValue> GetEnumValues()
		{
			if (!NativeType.IsEnum) yield break;
			foreach (string value in Enum.GetNames(NativeType))
			{
				yield return new RantModeValue(Util.CamelToSnake(value),
					(NativeType.GetMember(value)[0].GetCustomAttributes(typeof(RantDescriptionAttribute), true).First() as
						RantDescriptionAttribute)?.Description ?? string.Empty);
			}
		}
	}
}