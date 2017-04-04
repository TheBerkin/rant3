using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Common
{
    internal static class CmdLine
    {
        private static readonly Dictionary<string, List<string>> Arguments = new Dictionary<string, List<string>>();
        private static readonly HashSet<string> Flags = new HashSet<string>();
        private static readonly List<string> Paths = new List<String>();

        /// <summary>
        /// Determines whether the user specified a question mark as the argument.
        /// </summary>
        public static readonly bool Help;

        static CmdLine()
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            int argc = args.Length;
            if (argc == 0) return;

            if (argc == 1 && args[0] == "?")
            {
                Help = true;
                return;
            }

            bool isProperty = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (isProperty)
                {
					var name = args[i - 1].TrimStart('-');
					if (Arguments.ContainsKey(name))
						Arguments[name].Add(args[i]);
					else
					{
						Arguments[name] = new List<string>();
						Arguments[name].Add(args[i]);
					}
                    isProperty = false;
                }
                else if (args[i].StartsWith("--"))
                {
                    Flags.Add(args[i].TrimStart('-'));
                }
                else if (args[i].StartsWith("-"))
                {
                    isProperty = true;
                }
                else
                {
                    Paths.Add(args[i]);
                }
            }
        }

        public static string[] GetPaths() => Paths.ToArray();

        public static string Property(string name)
        {
            List<string> args;
            if (!Arguments.TryGetValue(name.ToLower(), out args))
            {
				return "";
            }
			return args.First();
        }

        public static string Property(string name, string defaultValue)
        {
            List<string> args;
            return !Arguments.TryGetValue(name.ToLower(), out args) ? defaultValue : args.First();
        }

		public static IEnumerable<string> Properties(string name)
		{
			List<string> args;
			return Arguments.TryGetValue(name.ToLower(), out args) ? args : new List<string>();
		}

        public static bool Flag(string name) => Flags.Contains(name);
    }
}
