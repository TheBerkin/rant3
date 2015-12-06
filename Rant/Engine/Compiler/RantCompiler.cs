using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Syntax;
using Rant.Stringes;
using Rant.Engine.Compiler.Parselets;

namespace Rant.Engine.Compiler
{
    internal class RantCompiler
    {
        public static RantAction Compile(string sourceName, string source) => new RantCompiler(sourceName, source).Read();

        private readonly string source;
        private readonly string sourceName;
        private readonly TokenReader reader;
        private readonly RantExpressionCompiler expressionCompiler;

		internal bool HasModule = false;
		internal readonly RantModule Module;

        public RantCompiler(string sourceName, string source)
        {
			Module = new RantModule(sourceName);

            this.source = source;
            this.sourceName = sourceName;

            reader = new TokenReader(sourceName, RantLexer.GenerateTokens(sourceName, source.ToStringe()));
            expressionCompiler = new RantExpressionCompiler(sourceName, source, reader, this);

            Parselet.SetCompilerAndReader(this, reader);
        }

        public RantAction Read()
        {
            var output = new List<RantAction>();

            // NOTE: since parselets are more like containers for multiple parsers, maybe figure out something to stuff a stack full of rather than parselets
            //var parseletStack = new Stack<Parselet>();
            var enumeratorStack = new Stack<IEnumerator<Parselet>>();

            Token<R> token = null;

            while (!reader.End)
            {
                token = reader.ReadToken();

                if (token.ID == R.EOF)
                    goto done;

                var parselet = Parselet.GetParselet(token, output.Add);
                //parseletStack.Push(parselet);
                enumeratorStack.Push(parselet.Parse());

                top:
                while (enumeratorStack.Any())
                {
                    var enumerator = enumeratorStack.Peek();

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current == null)
                            break;

                        //parseletStack.Push(enumerator.Current);
                        enumeratorStack.Push(enumerator.Current.Parse());
                        goto top;
                    }

                    //parseletStack.Pop();
                    enumeratorStack.Pop();
                }
            }

            done:
            return new RASequence(output, token);
        }

        public RantAction ReadExpression() => expressionCompiler.Read();

        public RantFunctionInfo GetFunctionInfo(RantFunctionGroup group, int argc, Stringe from, Stringe to)
        {
            var func = group.GetFunction(argc);

            if (func == null)
                SyntaxError(Stringe.Between(from, to), $"No overload of function '{group.Name}' can take {argc} arguments");

            return func;
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
