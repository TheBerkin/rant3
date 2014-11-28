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
            foreach (var argKeyVal in Environment.GetCommandLineArgs().Where(arg => arg.StartsWith("-")).Select(arg => arg.TrimStart('-').Split(new[] { '=' }, 2)))
            {
                if (argKeyVal.Length == 2)
                {
                    Arguments[argKeyVal[0].ToLower().Trim()] = argKeyVal[1];
                }
                else
                {
                    Flags.Add(argKeyVal[0]);
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

        public static bool Flag(string name) => Flags.Contains(name);
    }
}
