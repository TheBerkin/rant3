using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal abstract class Parselet
    {
        // static stuff

        static bool loaded = false;
        static Dictionary<R, Parselet> parseletDict;
        static Parselet defaultParselet;
        static RantCompiler sCompiler; // the s is for static
        static TokenReader sReader;

        static Parselet()
        {
            Load();
        }

        public static void SetCompilerAndReader(RantCompiler c, TokenReader r)
        {
            sCompiler = c;
            sReader = r;
        }

        public static void Load(bool forceNewLoad = false)
        {
            if (loaded && !forceNewLoad)
                return;

            // scan the Compiler.Parselets namespace for all parselets, create instances of them store them in a dictionary
            // it's clean, it's super easy to extend. a single statement loads them all from the namespace and sets them up right
            // it's slow and may be a memory hog with many parselets
            // maybe create the instances of the parselets as needed?

            parseletDict = new Dictionary<R, Parselet>();

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                t.IsClass &&
                !t.IsNested && // for some reason makes sure the internal enumerable type isn't included
                !t.IsAbstract && // makes sure we don't load this base class
                t.Namespace == "Rant.Engine.Compiler.Parselets");

            foreach (var type in types)
            {
                if (type.GetCustomAttribute<DefaultParseletAttribute>() != null)
                {
                    if (defaultParselet != null)
                        throw new RantInternalException($"Cannot define {type.Name} as default parselet: {defaultParselet.GetType().Name} is already defined as default parselet");

                    defaultParselet = (Parselet)Activator.CreateInstance(type);
                    continue;
                }

                var instance = (Parselet)Activator.CreateInstance(type);
                parseletDict.Add(instance.Identifier, instance);
            }

            if (defaultParselet == null)
                throw new RantInternalException($"Missing default parselet");

            loaded = true;
        }

        public static Parselet GetParselet(Token<R> token, Action<RantAction> outputDelegate)
        {
            Parselet parselet;
            if (parseletDict.TryGetValue(token.ID, out parselet))
            {
                if (!parselet.MatchingCompilerAndReader(sCompiler, sReader))
                    parselet.InternalSetCompilerAndReader(sCompiler, sReader);

                if (outputDelegate == null)
                    throw new RantInternalException("Output delegate is null");

                if (token == null)
                    throw new RantInternalException("Token is null");

                parselet.PushOutputDelegate(outputDelegate);
                parselet.PushToken(token);
                return parselet;
            }

            return GetDefaultParselet(token, outputDelegate);
        }

        static Parselet GetDefaultParselet(Token<R> token, Action<RantAction> outputDelegate)
        {
            if (defaultParselet == null)
                throw new RantInternalException("DefaultParselet not set");

            if (!defaultParselet.MatchingCompilerAndReader(sCompiler, sReader))
                defaultParselet.InternalSetCompilerAndReader(sCompiler, sReader);

            if (outputDelegate == null)
                throw new RantInternalException("Output delegate is null");

            if (token == null)
                throw new RantInternalException("Token is null");

            defaultParselet.PushOutputDelegate(outputDelegate);
            defaultParselet.PushToken(token);
            return defaultParselet;
        }

        // instance stuff

        public abstract R Identifier { get; }

        Stack<Token<R>> tokens;
        Stack<Action<RantAction>> outputDelegates;

        protected RantCompiler compiler;
        protected TokenReader reader;

        protected Parselet()
        {
            tokens = new Stack<Token<R>>();
            outputDelegates = new Stack<Action<RantAction>>();
        }

        // NOTE: this way of passing in the proper token and output override is kinda bad and a hack of sorts. maybe improve?
        void PushToken(Token<R> token) => tokens.Push(token);
        void PushOutputDelegate(Action<RantAction> action) => outputDelegates.Push(action);

        bool MatchingCompilerAndReader(RantCompiler compiler, TokenReader reader) => this.compiler == compiler && this.reader == reader;
        void InternalSetCompilerAndReader(RantCompiler compiler, TokenReader reader)
        {
            if (compiler == null)
                throw new RantInternalException("Compiler is null");

            if (reader == null)
                throw new RantInternalException("Token reader is null");

            this.compiler = compiler;
            this.reader = reader;
        }

        public IEnumerator<Parselet> Parse()
        {
            if (!tokens.Any())
                throw new RantInternalException("Token stack is empty");

            if (!outputDelegates.Any())
                throw new RantInternalException("Output delegate stack is empty");

            foreach (var parselet in InternalParse(tokens.Pop()))
                yield return parselet;

            outputDelegates.Pop();
        }

        protected abstract IEnumerable<Parselet> InternalParse(Token<R> token);

        protected void AddToOutput(RantAction action)
        {
            if (!outputDelegates.Any())
                throw new RantInternalException("Output delegate stack is empty");

            outputDelegates.Peek()(action);
        }
    }
}
