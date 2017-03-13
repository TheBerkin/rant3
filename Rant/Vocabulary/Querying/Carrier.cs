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

using Rant.Core.IO;

namespace Rant.Vocabulary.Querying
{
    /// <summary>
    /// Represents information that can be used to synchronize query selections based on certain criteria.
    /// </summary>
    public sealed class Carrier
    {
        private readonly Dictionary<CarrierComponentType, HashSet<string>> _components;

        /// <summary>
        /// Creates an empty carrier.
        /// </summary>
        public Carrier()
        {
            _components = new Dictionary<CarrierComponentType, HashSet<string>>();
        }

        /// <summary>
        /// Returns how many of a certain carrier component type are assigned to the current instance.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetTypeCount(CarrierComponentType type)
        {
            HashSet<string> set;
            if (!_components.TryGetValue(type, out set)) return 0;
            return set.Count;
        }

        /// <summary>
        /// Adds a component of the specified type and name to the current instance.
        /// </summary>
        /// <param name="type">The type of carrier to add.</param>
        /// <param name="values">The names to assign to the component type.</param>
        public void AddComponent(CarrierComponentType type, params string[] values)
        {
            HashSet<string> set;
            if (!_components.TryGetValue(type, out set))
            {
                _components[type] = new HashSet<string>(values);
                return;
            }
            foreach (string value in values) set.Add(value);
        }

        /// <summary>
        /// Iterates through the current instances's components of the specified type.
        /// </summary>
        /// <param name="type">The type of component to iterate through.</param>
        /// <returns></returns>
        public IEnumerable<string> GetComponentsOfType(CarrierComponentType type)
        {
            HashSet<string> set;
            if (!_components.TryGetValue(type, out set)) yield break;
            foreach (string value in set)
                yield return value;
        }

        /// <summary>
        /// Retreives the total amount of all components.
        /// </summary>
        /// <returns>The total amount of all components.</returns>
        public int GetTotalCount()
        {
            int count = 0;
            foreach (CarrierComponentType component in Enum.GetValues(typeof(CarrierComponentType)))
                count += GetTypeCount(component);
            return count;
        }

        internal void Serialize(EasyWriter output)
        {
            output.Write(_components.Count);
            foreach (var kv in _components)
            {
                output.Write((byte)kv.Key);
                output.Write(kv.Value.Count);
                foreach (string compName in kv.Value)
                    output.Write(compName);
            }
        }

        internal void Deserialize(EasyReader input)
        {
            int typeCount = input.ReadInt32();
            for (int i = 0; i < typeCount; i++)
            {
                var type = input.ReadEnum<CarrierComponentType>();
                int num = input.ReadInt32();
                for (int j = 0; j < num; j++)
                    AddComponent(type, input.ReadString());
            }
        }
    }
}