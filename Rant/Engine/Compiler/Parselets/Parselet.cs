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
        static Dictionary<R, string> tokenTypeParseletNameDict; // nice name
        static Dictionary<string, Parselet> parseletNameDict;
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

            tokenTypeParseletNameDict = new Dictionary<R, string>();
            parseletNameDict = new Dictionary<string, Parselet>();

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
            Console.WriteLine($"{parseletNameDict.Count} parselets and {tokenTypeParseletNameDict.Count} token name associations loaded in {timer.ElapsedMilliseconds}ms");
#endif
        }

        public static Parselet GetParselet(Token<R> token, Action<RantAction> outputDelegate)
        {
            string parseletName = "";
            if (!tokenTypeParseletNameDict.TryGetValue(token.ID, out parseletName))
            {
                // passing an empty name to the parselet means it will try and use its default parselet implementation
                return GetDefaultParselet("", token, outputDelegate);
            }

            return GetParselet(parseletName, token, outputDelegate);
        }

        public static Parselet GetParselet(string parseletName, Token<R> token, Action<RantAction> outputDelegate)
        {
            Parselet parselet;
            if (parseletNameDict.TryGetValue(parseletName, out parselet))
            {
                if (!parselet.MatchingCompilerAndReader(sCompiler, sReader))
                    parselet.InternalSetCompilerAndReader(sCompiler, sReader);

                if (outputDelegate == null)
                    throw new RantInternalException("Output delegate is null.");

                if (token == null)
                    throw new RantInternalException("Token is null.");

                parselet.PushOutputDelegate(outputDelegate);
                parselet.PushToken(token);
                parselet.PushParseletName(parseletName);
                return parselet;
            }

            return GetDefaultParselet(parseletName, token, outputDelegate);
        }

        static Parselet GetDefaultParselet(string parseletName, Token<R> token, Action<RantAction> outputDelegate)
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
            defaultParselet.PushParseletName(parseletName);
            return defaultParselet;
        }

        // instance stuff

        delegate IEnumerable<Parselet> TokenParserDelegate(Token<R> token);

        Dictionary<string, TokenParserDelegate> tokenParserMethods;
        TokenParserDelegate defaultParserMethod;

        Stack<string> parseletNames; // maybe needs a better name
        Stack<Token<R>> tokens;
        Stack<Action<RantAction>> outputDelegates;

        protected RantCompiler compiler;
        protected TokenReader reader;

        protected Parselet()
        {
            tokenParserMethods = new Dictionary<string, TokenParserDelegate>();

            parseletNames = new Stack<string>();
            tokens = new Stack<Token<R>>();
            outputDelegates = new Stack<Action<RantAction>>();

            var methods =
                from method in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                let attrib = method.GetCustomAttribute<TokenParserAttribute>()
                let defaultAttrib = method.GetCustomAttribute<DefaultParserAttribute>()
                where attrib != null || defaultAttrib != null
                select new { Method = method, Attrib = attrib, IsDefault = defaultAttrib != null };

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

                // TODO: maybe throw internal exception if the method has both DefaultParser and TokenParser attributes?

                var parseletName = method.Method.Name;
                // this could be a default method so it may not have the TokenParser attribute
                if (method.Attrib != null && !String.IsNullOrWhiteSpace(method.Attrib.Name))
                    parseletName = method.Attrib.Name;

                if (method.IsDefault)
                {
                    if (defaultParserMethod != null)
                        throw new RantInternalException($"Default parser method already defined for '{GetType().Name}'");

                    // associate our default method with us
                    parseletNameDict.Add(parseletName, this);

                    defaultParserMethod = (TokenParserDelegate)method.Method.CreateDelegate(typeof(TokenParserDelegate), this);
                    continue;
                }

                Parselet existingParselet;
                if (parseletNameDict.TryGetValue(parseletName, out existingParselet))
                    throw new RantInternalException($"'{existingParselet.GetType().Name}' already has an implementation called '{parseletName}'");

                // associate our method with us
                parseletNameDict.Add(parseletName, this);
                tokenParserMethods.Add(parseletName, (TokenParserDelegate)method.Method.CreateDelegate(typeof(TokenParserDelegate), this));

                if (method.Attrib.TokenType != null)
                {
                    var type = method.Attrib.TokenType.Value;
                    var existingName = "";
                    if (tokenTypeParseletNameDict.TryGetValue(type, out existingName))
                        throw new RantInternalException($"'{existingName}' is already associated with {type}");

                    // associate the method's name with the token type
                    tokenTypeParseletNameDict.Add(type, parseletName);
                }
            }
        }

        // NOTE: this way of passing in the proper token, parselet name and output override is kinda bad and a hack of sorts. maybe improve?
        void PushToken(Token<R> token) => tokens.Push(token);
        void PushOutputDelegate(Action<RantAction> action) => outputDelegates.Push(action);
        void PushParseletName(string parseletName) => parseletNames.Push(parseletName);

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
            if (!parseletNames.Any())
                throw new RantInternalException("Parselet name stack is empty.");

            if (!tokens.Any())
                throw new RantInternalException("Token stack is empty.");

            if (!outputDelegates.Any())
                throw new RantInternalException("Output delegate stack is empty.");

            var name = parseletNames.Pop();
            var token = tokens.Pop();

            TokenParserDelegate tokenParser = null;
            if (!tokenParserMethods.TryGetValue(name, out tokenParser))
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
