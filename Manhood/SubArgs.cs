using System;
using System.Collections.Generic;

namespace Manhood
{
    internal class SubArgs
    {
        private readonly Dictionary<string, string> argMap;

        public SubArgs(Interpreter interpreter, Subroutine sub, string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");

            if (args.Length != sub.Parameters.Length)
            {
                throw new ArgumentException("Parameter mismatch at subroutine '" + sub.Name + "': expected " + sub.Parameters.Length + ", got " + args.Length);
            }

            argMap = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                argMap[sub.Parameters[i].Name.ToLower()] = sub.Parameters[i].Interpreted
                    ? interpreter.Evaluate(args[i])
                    : args[i];
            }
        }

        public string GetArg(string name)
        {
            string arg;
            return argMap.TryGetValue(name.ToLower(), out arg) ? arg : "(MISSING ARG)";
        }
    }
}