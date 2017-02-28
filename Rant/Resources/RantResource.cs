#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

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