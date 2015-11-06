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
    internal class NewRantCompiler
    {
        readonly string source;
        readonly string sourceName;
        readonly TokenReader reader;
        readonly Stack<RantFunctionGroup> funcCalls = new Stack<RantFunctionGroup>();
        readonly Stack<Regex> regexes = new Stack<Regex>();
        readonly Stack<RASubroutine> subroutines = new Stack<RASubroutine>();
        readonly RantExpressionCompiler expressionCompiler;

        List<RantAction> output;

        public NewRantCompiler(string sourceName, string source)
        {
            this.source = source;
            this.sourceName = sourceName;

            reader = new TokenReader(sourceName, RantLexer.GenerateTokens(sourceName, source.ToStringe()));
            expressionCompiler = new RantExpressionCompiler(sourceName, source, reader, this);

            output = new List<RantAction>();

            Parselet.SetCompilerAndReader(this, reader);
        }

        public RantAction Read()
        {
            var parseletStack = new Stack<Parselet>();
            var enumeratorStack = new Stack<IEnumerator<Parselet>>();

            Token<R> token = null;

            while (!reader.End)
            {
                token = reader.ReadToken();
                var parselet = Parselet.GetParselet(token);
                parseletStack.Push(parselet);
                enumeratorStack.Push(parselet.Parse(null));

                top:
                while (enumeratorStack.Any())
                {
                    var enumerator = enumeratorStack.Peek();

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current == null)
                            break;

                        parseletStack.Push(enumerator.Current);
                        enumeratorStack.Push(enumerator.Current.Parse(null));
                        goto top;
                    }

                    parseletStack.Pop();
                    enumeratorStack.Pop();
                }
            }

            //done:
            return new RASequence(output, token);
        }

        public RantAction ReadExpression() => expressionCompiler.Read();

        public void AddToOutput(RantAction action) => output.Add(action);

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
