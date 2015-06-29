using System;
using System.Collections.Generic;
using System.Linq;

namespace Rave
{
    internal static class CmdLine
    {
        private static readonly Dictionary<string, string> Arguments = new Dictionary<string, string>();
        private static readonly HashSet<string> Flags = new HashSet<string>();
        private static readonly List<string> Paths = new List<String>();
		
	    public static readonly string Command = String.Empty;

        static CmdLine()
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            int argc = args.Length;
            if (argc == 0) return;

			Command = args[0].ToLower().Trim();

			bool isProperty = false;

            for (int i = 1; i < args.Length; i++)
            {
                if (isProperty)
                {
                    Arguments[args[i - 1].TrimStart('-')] = args[i];
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
            string arg;
            if (!Arguments.TryGetValue(name.ToLower(), out arg))
            {
                arg = "";
            }
            return arg;
        }

        public static string Property(string name, string defaultValue)
        {
            string arg;
            return !Arguments.TryGetValue(name.ToLower(), out arg) ? defaultValue : arg;
        }

        public static bool Flag(string name) => Flags.Contains(name);
    }
}
