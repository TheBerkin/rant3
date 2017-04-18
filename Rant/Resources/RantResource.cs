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

using Rant.Vocabulary;
using Rant.Core.IO;
using System.Text;
using System.IO;

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

        internal static RantResource DeserializeResource(EasyReader reader)
        {
			var typeCode = Encoding.ASCII.GetString(reader.ReadBytes(4));

			if (!_resourceTypeRegistry.TryGetValue(typeCode, out Type type))
				throw new InvalidDataException($"Unrecognized resource type: '{typeCode}'");

            var resource = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[0], null) as RantResource;

			resource?.DeserializeData(reader);

			return resource;
        }

        internal static void SerializeResource(RantResource resource, EasyWriter writer)
        {
			if (!_resourceIdRegistry.TryGetValue(resource.GetType(), out string rstr))
				throw new ArgumentException($"Resource type '{resource.GetType()}' is not registered.");

			writer.WriteBytes(Encoding.ASCII.GetBytes(rstr));
			resource.SerializeData(writer);
        }

        internal abstract void DeserializeData(EasyReader reader);
        internal abstract void SerializeData(EasyWriter writer);
        internal abstract void Load(RantEngine engine);
    }
}