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

#if DEBUG
            System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
#endif

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
                // here we just hope that the parselet handles itself properly
            }

            if (defaultParselet == null)
                throw new RantInternalException($"Missing default parselet");

            loaded = true;

#if DEBUG
            timer.Stop();
            Console.WriteLine($"Parselet loading: {timer.ElapsedMilliseconds}ms");
#endif
        }

        public static Parselet GetParselet(Token<R> token, Action<RantAction> outputDelegate)
        {
            Parselet parselet;
            if (parseletDict.TryGetValue(token.ID, out parselet))
            {
                if (!parselet.MatchingCompilerAndReader(sCompiler, sReader))
                    parselet.InternalSetCompilerAndReader(sCompiler, sReader);

                if (outputDelegate == null)
                    throw new RantInternalException("Output delegate is null.");

                if (token == null)
                    throw new RantInternalException("Token is null.");

                parselet.PushOutputDelegate(outputDelegate);
                parselet.PushToken(token);
                return parselet;
            }

            return GetDefaultParselet(token, outputDelegate);
        }

        static Parselet GetDefaultParselet(Token<R> token, Action<RantAction> outputDelegate)
        {
            if (defaultParselet == null)
                throw new RantInternalException("DefaultParselet not set.");

            if (!defaultParselet.MatchingCompilerAndReader(sCompiler, sReader))
                defaultParselet.InternalSetCompilerAndReader(sCompiler, sReader);

            if (outputDelegate == null)
                throw new RantInternalException("Output delegate is null.");

            if (token == null)
                throw new RantInternalException("Token is null.");

            defaultParselet.PushOutputDelegate(outputDelegate);
            defaultParselet.PushToken(token);
            return defaultParselet;
        }

        // instance stuff

        delegate IEnumerable<Parselet> TokenParserDelegate(Token<R> token);

        Dictionary<R, TokenParserDelegate> tokenParserMethods;
        TokenParserDelegate defaultParserMethod;

        Stack<Token<R>> tokens;
        Stack<Action<RantAction>> outputDelegates;

        protected RantCompiler compiler;
        protected TokenReader reader;

        protected Parselet()
        {
            tokenParserMethods = new Dictionary<R, TokenParserDelegate>();

            tokens = new Stack<Token<R>>();
            outputDelegates = new Stack<Action<RantAction>>();

            var methods =
                from method in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                let attrib = method.GetCustomAttribute<TokenParserAttribute>()
                let defaultAttrib = method.GetCustomAttribute<DefaultParserAttribute>()
                where attrib != null || defaultAttrib != null
                select new { Method = method, TokenType = attrib?.TokenType, IsDefault = defaultAttrib != null };

            if (!methods.Any())
                throw new RantInternalException($"'{GetType().Name}' doesn't contain any parselet implementations.");

            foreach (var method in methods)
            {
                if (method.Method.IsStatic)
                    throw new RantInternalException($"Parselet method musn't be static: '{method.Method.Name}'.");

                if (method.Method.IsGenericMethod)
                    throw new RantInternalException($"Parselet method musn't be generic: '{method.Method.Name}'.");

                if (method.Method.GetParameters().Length != 1)
                    throw new RantInternalException($"Invalid amount of parameters for parselet method: '{method.Method.Name}'.");

                if (method.Method.GetParameters().First().ParameterType != typeof(Token<R>))
                    throw new RantInternalException($"Wrong parameter type for parselet method: '{method.Method.Name}'.");

                if (method.IsDefault)
                {
                    if (defaultParserMethod != null)
                        throw new RantInternalException($"Default parser method already defined for '{GetType().Name}'");

                    defaultParserMethod = (TokenParserDelegate)method.Method.CreateDelegate(typeof(TokenParserDelegate), this);
                    continue;
                }

                Parselet existingParselet;
                if (parseletDict.TryGetValue(method.TokenType.Value, out existingParselet))
                    throw new RantInternalException($"'{existingParselet.GetType().Name}' already has an implementation for {method.TokenType}");

                parseletDict.Add(method.TokenType.Value, this);
                tokenParserMethods.Add(method.TokenType.Value, (TokenParserDelegate)method.Method.CreateDelegate(typeof(TokenParserDelegate), this));
            }
        }

        // NOTE: this way of passing in the proper token and output override is kinda bad and a hack of sorts. maybe improve?
        void PushToken(Token<R> token) => tokens.Push(token);
        void PushOutputDelegate(Action<RantAction> action) => outputDelegates.Push(action);

        bool MatchingCompilerAndReader(RantCompiler compiler, TokenReader reader) => this.compiler == compiler && this.reader == reader;
        void InternalSetCompilerAndReader(RantCompiler compiler, TokenReader reader)
        {
            if (compiler == null)
                throw new RantInternalException("Compiler is null.");

            if (reader == null)
                throw new RantInternalException("Token reader is null.");

            this.compiler = compiler;
            this.reader = reader;
        }

        public IEnumerator<Parselet> Parse()
        {
            if (!tokens.Any())
                throw new RantInternalException("Token stack is empty.");

            if (!outputDelegates.Any())
                throw new RantInternalException("Output delegate stack is empty.");

            var token = tokens.Pop();

            TokenParserDelegate tokenParser = null;
            if (!tokenParserMethods.TryGetValue(token.ID, out tokenParser))
            {
                if (defaultParserMethod == null)
                    throw new RantInternalException($"Parselet '{GetType().Name}' doesn't contain an implementation for {token.ID}.");

                tokenParser = defaultParserMethod;
            }

            foreach (var parselet in tokenParser(token))
                yield return parselet;

            outputDelegates.Pop();
        }

        protected void AddToOutput(RantAction action)
        {
            if (!outputDelegates.Any())
                throw new RantInternalException("Output delegate stack is empty.");

            outputDelegates.Peek()(action);
        }
    }
}
