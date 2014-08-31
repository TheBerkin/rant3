using System;
using System.Collections.Generic;

using Manhood.Compiler;

using Stringes;

namespace Manhood
{
    internal partial class Interpreter
    {
        private static readonly Dictionary<string, TagFunc> TagFuncs;

        static Interpreter()
        {
            TagFuncs = new Dictionary<string, TagFunc>
            {
                {"rep", new TagFunc(1, (interpreter, source, tagName, args) =>
                {
                    int num;
                    if (!Int32.TryParse(args[0], out num))
                    {
                        throw new ManhoodException(source, tagName, "Invalid repetition value '" + args[0] + "' - must be a number.");
                    }
                    if (num < 0)
                    {
                        throw new ManhoodException(source, tagName, "Repetition value cannot be negative.");
                    }

                    interpreter.PendingBlockAttribs.Repetitons = num;
                })}
            };
        }

        private class TagFunc
        {
            private readonly int _paramCount;
            private readonly Action<Interpreter, Source, Stringe, string[]> _func;

            public TagFunc(int paramCount, Action<Interpreter, Source, Stringe, string[]> func)
            {
                _paramCount = paramCount;
                _func = func;
            }

            public void Invoke(Interpreter interpreter, Source source, Stringe tagName, string[] args)
            {
                if (args.Length != _paramCount)
                    throw new ManhoodException(source, tagName, "Tag '" + tagName.Value + "' expected " + _paramCount + " " + (_paramCount == 1 ? "argument" : "arguments") + " but got " + args.Length + ".");
                _func(interpreter, source, tagName, args);
            }
        }
    }
}