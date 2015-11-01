using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Rant.Engine.Syntax;
using Rant.Vocabulary;
using Rant.Stringes;
using Rant.Engine.Compiler.Parselets;

namespace Rant.Engine.Compiler
{
    public enum ReadType
    {
        /// <summary>
        /// Reads a list of items and returns an RASequence.
        /// </summary>
        Sequence,

        /// <summary>
        /// Reads a list of items and returns an RABlock.
        /// </summary>
        Block,

        /// <summary>
        /// Reads a list of arguments and returns an RAFunction.
        /// </summary>
        FuncArgs,

        /// <summary>
        /// Reads a query and returns a RAQuery
        /// </summary>
        Query,

        /// <summary>
        /// Reads a query carrier
        /// </summary>
        QueryCarrier,

        /// <summary>
        /// Reads the arguments needed by a replacer and return an RAReplacer.
        /// </summary>
        ReplacerArgs,

        /// <summary>
        /// Reads the arguments needed by a subroutine.
        /// </summary>
        SubroutineArgs,

        /// <summary>
        /// Reads the body of a subroutine.
        /// </summary>
        SubroutineBody,

        /// <summary>
        /// Reads a block weight.
        /// </summary>
        BlockWeight
    }

    internal class NewRantCompiler
    {
        readonly string source;
        readonly string sourceName;
        readonly TokenReader reader;
        readonly Stack<RantFunctionGroup> funcCalls = new Stack<RantFunctionGroup>();
        readonly Stack<Regex> regexes = new Stack<Regex>();
        readonly Stack<RASubroutine> subroutines = new Stack<RASubroutine>();
        Query query;
        readonly RantExpressionCompiler expressionCompiler;

        List<RantAction> output;

        public NewRantCompiler(string sourceName, string source)
        {
            this.source = source;
            this.sourceName = sourceName;

            reader = new TokenReader(sourceName, RantLexer.GenerateTokens(sourceName, source.ToStringe()));
            expressionCompiler = new RantExpressionCompiler(sourceName, source, reader, this);

            output = new List<RantAction>();
        }

        public RantAction Read(Token<R> fromToken = null)
        {
            var parseletStack = new Stack<Parselet>();
            var enumeratorStack = new Stack<IEnumerator<Parselet>>();

            Token<R> token = null;

            while (!reader.End)
            {
                token = reader.ReadToken();
                var parselet = Parselet.FromTokenID(token.ID);
                parseletStack.Push(parselet);
                enumeratorStack.Push(parselet.Parse(this, reader, token, fromToken));

                top:
                while (enumeratorStack.Any())
                {
                    var enumerator = enumeratorStack.Peek();

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current == null)
                            break;

                        fromToken = parseletStack.Peek().FromToken;
                        parseletStack.Push(enumerator.Current);
                        enumeratorStack.Push(enumerator.Current.Parse(this, reader, token, fromToken));
                        goto top;
                    }

                    parseletStack.Pop();
                    enumeratorStack.Pop();
                }
            }

            //done:
            return new RASequence(output, token);
        }

        public void AddToOutput(RantAction action) => output.Add(action);
        public void PushToFuncCalls(RantFunctionGroup group) => funcCalls.Push(group);
        public void PushToRegexes(Regex regex) => regexes.Push(regex);
        public void PushToSubroutines(RASubroutine sub) => subroutines.Push(sub);

        public Query GetQuery() => query;
        public void SetNewQuery(Query query) => this.query = query;
        public void SetQueryExclusivity(bool value) => query.Exclusive = value;
        public void SetQuerySubtype(string value) => query.Subtype = value;
        public void AddRuleSwitchToQuery(params ClassFilterRule[] rules) => query.ClassFilter.AddRuleSwitch(rules);
        public void SetQuerySyllablePredicate(Range value) => query.SyllablePredicate = value;
        public void AddQueryCarrierComponent(CarrierComponent type, params string[] values) => query.Carrier.AddComponent(type, values);

        public RantFunctionInfo GetFunctionInfo(RantFunctionGroup group, int argc, Stringe from, Stringe to)
        {
            var func = group.GetFunction(argc);

            if (func == null)
                SyntaxError(Stringe.Between(from, to), $"No overload of function '{group.Name}' can take {argc} arguments");

            return func;
        }

        public void Unexpected(Stringe token)
        {
            throw new RantCompilerException(sourceName, token, $"Unexpected token: '{token.Value}'");
        }

        public void SyntaxError(Stringe token, string message)
        {
            throw new RantCompilerException(sourceName, token, message);
        }

        public void SyntaxError(Stringe token, Exception innerException)
        {
            throw new RantCompilerException(sourceName, token, innerException);
        }
    }
}
