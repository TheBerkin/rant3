using System;
using System.Collections.Generic;

namespace Rant.Vocabulary.Querying
{
    /// <summary>
    /// Represents information that can be used to synchronize query selections based on certain criteria.
    /// </summary>
    public sealed class Carrier
    {
        private readonly Dictionary<CarrierComponent, HashSet<string>> _components;

        /// <summary>
        /// Creates an empty carrier.
        /// </summary>
        public Carrier()
        {
            _components = new Dictionary<CarrierComponent, HashSet<string>>();
        }

        /// <summary>
        /// Returns how many of a certain carrier component type are assigned to the current instance.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetTypeCount(CarrierComponent type)
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
        public void AddComponent(CarrierComponent type, params string[] values)
        {
            HashSet<string> set;
            if (!_components.TryGetValue(type, out set))
            {
                _components[type] = new HashSet<string>(values);
                return;
            }
            foreach (var value in values) set.Add(value);
		}

        /// <summary>
        /// Iterates through the current instances's carriers of the specified type.
        /// </summary>
        /// <param name="type">The type of component to iterate through.</param>
        /// <returns></returns>
        public IEnumerable<string> GetCarriers(CarrierComponent type)
        {
            HashSet<string> set;
            if (!_components.TryGetValue(type, out set)) yield break;
            foreach(var value in set)
            {
                yield return value;
            }
        }

		/// <summary>
		/// Retreives the total amount of all components.
		/// </summary>
		/// <returns>The total amount of all components.</returns>
		public int GetTotalCount()
		{
			int count = 0;
			foreach (CarrierComponent component in Enum.GetValues(typeof(CarrierComponent)))
				count += GetTypeCount(component);
			return count;
        }
    }
}