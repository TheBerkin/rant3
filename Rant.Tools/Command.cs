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
using System.Linq;
using System.Reflection;

namespace Rant.Tools
{
    internal abstract class Command
    {
        private static readonly Dictionary<string, Command> _cmdMap;
        private readonly List<CommandParamAttribute> _params = new List<CommandParamAttribute>();
        private readonly List<CommandFlagAttribute> _flags = new List<CommandFlagAttribute>();
        private string _name, _desc;
        private bool _requiresPath, _usesPath;

        public string Name => _name;
        public string Description => _desc;

        static Command()
        {
            _cmdMap = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Command))))
            {
                var nameAttr = type.GetCustomAttribute<CommandNameAttribute>();
                if (nameAttr == null) continue;
                var cmd = _cmdMap[nameAttr.Name] = Activator.CreateInstance(type) as Command;
                cmd._name = nameAttr.Name;
                cmd._desc = nameAttr.Description;
                cmd._params.AddRange(type.GetCustomAttributes<CommandParamAttribute>());
                cmd._flags.AddRange(type.GetCustomAttributes<CommandFlagAttribute>());
                cmd._requiresPath = nameAttr.RequireFilePath;
                cmd._usesPath = nameAttr.UsesPath;
            }
        }

        public static void Run(string cmdname)
        {
            if (String.IsNullOrWhiteSpace(cmdname))
            {
                Console.WriteLine($"\nRant Command Line Tools for Rant {typeof(RantEngine).Assembly.GetName().Version.ToString(3)}\n");
                Console.WriteLine($"Usage: {Program.Name} <command> [args]\n");
                Console.WriteLine("Use the --help option on any command to display its help text.\n");
                Console.WriteLine("Available commands: ");
                foreach (var c in _cmdMap.Values.OrderBy(c => c.Name))
                {
                    Console.WriteLine($"  {c.Name}\t\t{c.Description}");
                }
                Console.WriteLine();
                return;
            }

            if (!_cmdMap.TryGetValue(cmdname, out Command cmd))
            {
                Console.WriteLine($"Command '{cmdname}' does not exist.");
            }

            if (CmdLine.Flag("h") || CmdLine.Flag("help"))
            {
                GetHelpText(cmd);
                return;
            }

            if (cmd._requiresPath && CmdLine.GetPaths().Length == 0)
            {
                throw new ArgumentException($"The command '{cmd.Name}' requires a path.");
            }

            var missing = cmd._params.Where(p => p.Required && String.IsNullOrEmpty(CmdLine.Property(p.Name))).ToArray();
            if (missing.Length > 0)
            {
                throw new ArgumentException($"The command '{cmd.Name}' is missing the following required properties: {missing.Aggregate(missing[0].Name, (c, n) => c + ", " + n.Name)}");
            }

            foreach (var p in cmd._params.Where(p => p.Required))
            {
                if (String.IsNullOrEmpty(CmdLine.Property(p.Name)))
                {
                    throw new ArgumentException($"The command '{cmd.Name}' requires a -{p.Name} parameter.");
                }
            }

            cmd.OnRun();
        }

        public static void GetHelpText(string cmdname)
        {
            if (!_cmdMap.TryGetValue(cmdname, out Command cmd))
            {
                Console.WriteLine($"Command '{cmdname}' does not exist.");
            }

            GetHelpText(cmd);
        }

        public static void GetHelpText(Command cmd)
        {
            Console.WriteLine();
            Console.WriteLine(cmd.Name.ToUpperInvariant());
            Console.WriteLine();
            Console.WriteLine(cmd.Description);

            var pRequired = cmd._params.Where(p => p.Required).Select(p => $"-{p.Name} ...").ToArray();
            var pOptional = cmd._params.Where(p => !p.Required).Select(p => $"[-{p.Name} ...]").ToArray();
            var flags = cmd._flags.Select(f => $"[--{f.Name}]").ToArray();

            Console.Write($"\nUsage: {Program.Name} {cmd.Name}");
            if (pRequired.Length > 0) Console.Write($" {pRequired.Aggregate((c, n) => c + " " + n)}");
            if (pOptional.Length > 0) Console.Write($" {pOptional.Aggregate((c, n) => c + " " + n)}");
            if (flags.Length > 0) Console.Write($" {flags.Aggregate((c, n) => c + " " + n)}");
            if (cmd._requiresPath || cmd._usesPath) Console.Write(cmd._requiresPath ? " path" : " [path]");
            Console.WriteLine();

            Console.WriteLine("\nParameters:");
            if (cmd._params.Count == 0 && cmd._flags.Count == 0)
            {
                Console.WriteLine("(None)");
            }
            else
            {
                foreach (var p in cmd._params)
                {
                    Console.WriteLine("{0, -32} {1}", $"  -{p.Name}{(p.Required ? " (Required) " : " ")}", p.Description);
                }

                foreach (var f in cmd._flags)
                {
                    Console.WriteLine("{0, -32} {1}", $"  --{f.Name}", f.Description);
                }
            }
            Console.WriteLine();
        }

        protected abstract void OnRun();
    }
}