using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal abstract class Parselet
    {
        // static stuff

        static Dictionary<R, Parselet> parseletDict;
        static Parselet defaultParselet;

        static Parselet()
        {
            // scan the Compiler.Parselets namespace for all parselets, create instances of them store them in a dictionary
            // it's clean, it's super easy to extend. a single statement loads them all from the namespace and sets them up right
            // it's slow-ish and may be a memory hog with many parselets
            // maybe create the instances of the parselets as needed?

            parseletDict = new Dictionary<R, Parselet>();

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                t.IsClass &&
                !t.IsNested && // for some reason makes sure some weird type isn't included
                !t.IsAbstract && // makes sure we don't load this base class
                t.Namespace == "Rant.Engine.Compiler.Parselets");

            foreach (var type in types)
            {
                if (type.GetCustomAttribute<DefaultParseletAttribute>() != null)
                {
                    if (defaultParselet != null)
                        throw new RantInternalException($"{defaultParselet.GetType().Name} is already defined as default parselet");

                    defaultParselet = (Parselet)Activator.CreateInstance(type);
                    continue;
                }

                var instance = (Parselet)Activator.CreateInstance(type);
                parseletDict.Add(instance.Identifier, instance);
            }
        }

        public static Parselet FromTokenID(R id)
        {
            Parselet parselet;
            if (parseletDict.TryGetValue(id, out parselet))
                return parselet;

            return defaultParselet;
        }

        // instance stuff

        public abstract R Identifier { get; }

        public Parselet()
        {
        }

        // TODO: return regexes and funcgroups
        public abstract IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, ReadType readType, Token<R> token);
    }
}
