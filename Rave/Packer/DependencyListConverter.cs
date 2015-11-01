using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Rant;

namespace Rave.Packer
{
	public class DependencyListConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var array = JArray.Load(reader);
			var list = new List<RantPackageDependency>();
			foreach (var item in array)
			{
				var obj = item as JObject;
				if (obj == null) continue;
				var tId = obj["id"] as JValue;
				if (tId == null) continue;
				var tVersion = obj["version"] as JValue;
				if (tVersion == null) continue;
				var tAllowNewer = obj["allow-newer"] as JValue;
				var dep = new RantPackageDependency(tId.Value.ToString(), tVersion.Value.ToString());
				if (tAllowNewer != null && tAllowNewer.Type == JTokenType.Boolean)
					dep.AllowNewer = (bool)tAllowNewer.Value;
				list.Add(dep);
			}
			return list;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(IEnumerable<RantPackageDependency>);
		}

		public override bool CanWrite => false;
	}
}