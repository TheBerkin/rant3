using System;
using System.Collections.Generic;

namespace Rant
{
    /// <summary>
    /// Represents a collection of function hooks which Rant patterns can call using the [extern] function.
    /// </summary>
    public class HookCollection
    {
        private readonly Dictionary<string, Func<string>> _hooks = new Dictionary<string, Func<string>>();
        private readonly HashSet<Func<string>> _funcs = new HashSet<Func<string>>(); 

        internal string Call(string name)
        {
            Func<string> func;
            return !_hooks.TryGetValue(name, out func) ? null : func();
        }

        /// <summary>
        /// Adds a function to the collection with the specified name. The hook name can only contain letters, decimal digits, and underscores.
        /// </summary>
        /// <param name="name">The name of the function hook.</param>
        /// <param name="func">The function associated with the hook.</param>
        public void AddHook(string name, Func<string> func)
        {
            if (!Util.ValidateName(name)) throw new FormatException("Hook name can only contain letters, decimal digits, and underscores.");
            _hooks[name] = func;
            _funcs.Add(func);
        }

        /// <summary>
        /// Determines whether the HookCollection object contains a hook with the specified name.
        /// </summary>
        /// <param name="name">The name of the hook to search for.</param>
        /// <returns></returns>
        public bool HasHook(string name)
        {
            return _hooks.ContainsKey(name);
        }

        /// <summary>
        /// Determines whether the HookCollection object contains a hook with the specified function.
        /// </summary>
        /// <param name="func">The function to search for.</param>
        /// <returns></returns>
        public bool HasHook(Func<string> func)
        {
            return _funcs.Contains(func);
        }

        /// <summary>
        /// Removes the hook with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the hook to remove.</param>
        public void RemoveHook(string name)
        {
            Func<string> func;
            if (_hooks.TryGetValue(name, out func))
            {
                _funcs.Remove(func);
            }
            _hooks.Remove(name);
        }
    }
}