using System;
using System.Collections.Generic;
using System.Linq;

namespace RantConsole
{
    static class CmdLine
    {
        private static readonly Dictionary<string, string> Arguments = new Dictionary<string, string>();
        private static readonly HashSet<string> Flags = new HashSet<string>();

        static CmdLine()
        {
            var args = Environment.GetCommandLineArgs();
            int argc = args.Length;
            if (argc == 0) return;

            bool isProperty = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (isProperty)
                {
                    if (i == argc - 1) break;
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
            }
        }

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
