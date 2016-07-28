using System;
using System.Collections.Generic;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Parsing
{
	internal class SequenceParser : Parser
	{
		public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RantAction> actionCallback)
		{
			Token<R> token;

			while (!reader.End)
			{
				token = reader.ReadToken();

				switch (token.ID)
				{
					case R.LeftAngle:
						yield return Get<QueryParser>();
						break;

					case R.LeftSquare:
						// TODO: Tags
						break;

					case R.LeftCurly:
						reader.SkipSpace();
						// Tell the compiler that we're about to read a block
						compiler.SetNextContext(CompileContext.BlockSequence);
						yield return Get<SequenceParser>();
						break;

					case R.Pipe:
						if (context == CompileContext.BlockSequence)
						{
							// TODO: Complete the element and start reading the next one
						}
						else
						{
							goto default; // Print it if we're not in a block
						}
						break;

					case R.RightCurly:
						if (context == CompileContext.BlockSequence)
						{
							// TODO: Complete the element and terminate the block
						}
						else
						{
							compiler.SyntaxError(token, "Unexpected block terminator");
						}
						break;

					case R.Whitespace:
						switch (context)
						{
							case CompileContext.DefaultSequence:
								actionCallback(new RAText(token));
								break;
							case CompileContext.BlockSequence:
								switch (reader.PeekType())
								{
									case R.Pipe:
									case R.RightCurly:
										continue; // Ignore whitespace at the end of block elements
								}
								break;
						}
						break;
						
					case R.EscapeSequence: // Handle escape sequences
						actionCallback(new RAEscape(token));
						break;

					case R.EOF:
						yield break;

					default: // Handle text
						actionCallback(new RAText(token));
						break;
				}
			}
		}
	}
}