using System;
using System.Collections.Generic;
using System.Reflection;

using Rant.Core.IO.Bson;
using Rant.Vocabulary;

namespace Rant.Resources
{
	/// <summary>
	/// The base class for Rant resources that can be included in a package.
	/// </summary>
	public abstract class RantResource
	{
		private static readonly Dictionary<string, Type> _resourceTypeRegistry = new Dictionary<string, Type>();
		private static readonly Dictionary<Type, string> _resourceIdRegistry = new Dictionary<Type, string>();

		static RantResource()
		{
			RegisterType<RantDictionaryTable>("dic2");
			RegisterType<RantProgram>("prog");
		}

		internal static void RegisterType<T>(string id) where T : RantResource
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			_resourceTypeRegistry[id] = typeof(T);
			_resourceIdRegistry[typeof(T)] = id;
		}

		internal static RantResource DeserializeResource(BsonItem dataHeader)
		{
			Type type;
			BsonItem dataItem;
			if (!_resourceTypeRegistry.TryGetValue(dataHeader["type"], out type) || (dataItem = dataHeader["data"]) == null)
				return null;

			var resource = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[0], null) as RantResource;

			resource?.DeserializeData(dataItem);

			return resource;
		}

		internal static BsonItem SerializeResource(RantResource resource)
		{
			string rstr;
			if (!_resourceIdRegistry.TryGetValue(resource.GetType(), out rstr)) return null;
			var res = new BsonItem
			{
				["type"] = rstr,
				["data"] = resource.SerializeData()
			};
			return res;
		}

		internal abstract void DeserializeData(BsonItem data);
		internal abstract BsonItem SerializeData();
		internal abstract void Load(RantEngine engine);
	}
}