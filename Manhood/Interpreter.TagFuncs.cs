using System;
using System.Collections.Generic;
using System.Linq;

using Manhood.Compiler;

using Stringes;

namespace Manhood
{
    // The return value is returned by the blueprint that executes the tag. If it's true, the interpreter will skip to the top of the state stack.
    internal delegate bool TagFunc(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args);

    internal partial class Interpreter
    {
        internal static readonly Dictionary<string, TagDef> TagFuncs;

        static Interpreter()
        {
            TagFuncs = new Dictionary<string, TagDef>();

            TagFuncs["rep"] = TagFuncs["r"] = new TagDef(Repeat, TagArgType.Result);
            TagFuncs["num"] = TagFuncs["n"] = new TagDef(Number, TagArgType.Result, TagArgType.Result);
            TagFuncs["sep"] = TagFuncs["s"] = new TagDef(Separator, TagArgType.Tokens);
            TagFuncs["before"] = new TagDef(Before, TagArgType.Tokens);
            TagFuncs["after"] = new TagDef(After, TagArgType.Tokens);
            TagFuncs["chance"] = new TagDef(Chance, TagArgType.Result);
        }

        private static bool Chance(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            int a;
            if (!Int32.TryParse(args[0].GetString(), out a))
            {
                throw new ManhoodException(source, tagname, "Invalid chance number.");
            }
            interpreter.SetChance(a);
            return false;
        }

        private static bool After(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            interpreter.PendingBlockAttribs.After = args[0].GetTokens();
            return false;
        }

        private static bool Before(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            interpreter.PendingBlockAttribs.Before = args[0].GetTokens();
            return false;
        }

        private static bool Number(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            int a, b;
            if (!Int32.TryParse(args[0].GetString(), out a) || !Int32.TryParse(args[1].GetString(), out b))
            {
                throw new ManhoodException(source, tagName, "Range values could not be parsed. They must be numbers.");
            }
            interpreter.Print(interpreter.RNG.Next(a, b + 1));
            return false;
        }

        private static bool Separator(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            interpreter.PendingBlockAttribs.Separator = args[0].GetTokens();
            return false;
        }

        private static bool Repeat(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            var reps = args[0].GetString().ToLower().Trim();
            if (reps == "each")
            {
                interpreter.PendingBlockAttribs.Repetitons = Repeater.Each;
                return false;
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
            return false;
        }

        internal class TagDef
        {
            private readonly int _paramCount;
            private readonly TagArgType[] _argTypes;
            private readonly TagFunc _func;

            public TagArgType[] ArgTypes
            {
                get { return _argTypes; }
            }

            public TagDef(TagFunc func, params TagArgType[] argTypes)
            {
                _paramCount = argTypes.Length;
                _argTypes = argTypes;
                _func = func;
            }

            public bool Invoke(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
            {
                if (args.Length != _paramCount)
                    throw new ManhoodException(source, tagName, "Tag '" + tagName.Value + "' expected " + _paramCount + " " + (_paramCount == 1 ? "argument" : "arguments") + " but got " + args.Length + ".");
                return _func(interpreter, source, tagName, args);
            }
        }
    }
}