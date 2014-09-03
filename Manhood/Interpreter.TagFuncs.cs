using System;
using System.Collections.Generic;
using System.Linq;

using Manhood.Compiler;

using Stringes;

namespace Manhood
{
    internal partial class Interpreter
    {
        private static readonly Dictionary<string, TagDef> TagFuncs;

        static Interpreter()
        {
            TagFuncs = new Dictionary<string, TagDef>();

            TagFuncs["rep"] = TagFuncs["r"] = new TagDef(Repeat, TagArgType.Result);
            TagFuncs["num"] = TagFuncs["n"] = new TagDef(Number, TagArgType.Result, TagArgType.Result);
            TagFuncs["sep"] = TagFuncs["s"] = new TagDef(Separator, TagArgType.Tokens);
        }

        private static void Number(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            int a, b;
            if (!Int32.TryParse(args[0].GetString(), out a) || !Int32.TryParse(args[1].GetString(), out b))
            {
                throw new ManhoodException(source, tagName, "Range values could not be parsed. They must be numbers.");
            }
            interpreter.Print(interpreter.RNG.Next(a, b + 1));
        }

        private static void Separator(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            interpreter.PendingBlockAttribs.Separator = args[0].GetTokens();
        }

        private static void Repeat(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            var reps = args[0].GetString().ToLower().Trim();
            if (reps == "each")
            {
                interpreter.PendingBlockAttribs.Repetitons = Repeater.Each;
                return;
            }

            int num;
            if (!Int32.TryParse(reps, out num))
            {
                throw new ManhoodException(source, tagName, "Invalid repetition value '" + args[0] + "' - must be a number.");
            }
            if (num < 0)
            {
                throw new ManhoodException(source, tagName, "Repetition value cannot be negative.");
            }

            interpreter.PendingBlockAttribs.Repetitons = num;
        }

        internal delegate void TagFunc(Interpreter interpreter, Source source, Stringe name, TagArg[] args);

        private class TagDef
        {
            private readonly int _paramCount;
            private readonly TagArgType[] _argTypes;
            private readonly TagFunc _func;
            private readonly int _stringArgCount;
            private readonly int _tokenArgCount;

            public int StringArgCount
            {
                get { return _stringArgCount; }
            }

            public int TokenArgCount
            {
                get { return _tokenArgCount; }
            }

            public TagArgType[] ArgTypes
            {
                get { return _argTypes; }
            }

            public TagDef(TagFunc func, params TagArgType[] argTypes)
            {
                _paramCount = argTypes.Length;
                _stringArgCount = argTypes.Count(t => t == TagArgType.Result);
                _tokenArgCount = argTypes.Count(t => t == TagArgType.Tokens);
                _argTypes = argTypes;
                _func = func;
            }

            public void Invoke(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
            {
                if (args.Length != _paramCount)
                    throw new ManhoodException(source, tagName, "Tag '" + tagName.Value + "' expected " + _paramCount + " " + (_paramCount == 1 ? "argument" : "arguments") + " but got " + args.Length + ".");
                _func(interpreter, source, tagName, args);
            }
        }
    }
}