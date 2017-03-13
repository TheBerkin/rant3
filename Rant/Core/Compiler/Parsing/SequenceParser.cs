#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;

using Rant.Core.Compiler.Syntax;

namespace Rant.Core.Compiler.Parsing
{
    internal class SequenceParser : Parser
    {
        public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader,
            Action<RST> actionCallback)
        {
            Token token;

            while (!reader.End)
            {
                token = reader.ReadToken();

                switch (token.Type)
                {
                    case R.LeftAngle:
                        yield return Get<QueryParser>();
                        break;

                    case R.LeftSquare:
                        yield return Get<TagParser>();
                        break;

                    case R.LeftCurly:
                        reader.SkipSpace();
                        yield return Get<BlockParser>();
                        break;

                    case R.Pipe:
                        if (context == CompileContext.BlockSequence)
                            yield break;
                        goto default; // Print it if we're not in a block

                    case R.RightCurly:
                        if (context == CompileContext.BlockSequence)
                        {
                            compiler.LeaveContext();
                            yield break;
                        }
                        compiler.SyntaxError(token, false, "err-compiler-unexpected-token", token.Value);
                        break;

                    // end of argument
                    case R.Semicolon:
                        if (context == CompileContext.ArgumentSequence)
                            yield break;
                        // this is probably just a semicolon in text
                        actionCallback(new RstText(token.Value, token.ToLocation()));
                        break;

                    case R.RightSquare:
                        // end of arguments / direct object in query
                        switch (context)
                        {
                            case CompileContext.ArgumentSequence:
                            case CompileContext.SubroutineBody:
                            case CompileContext.QueryComplement:
                                compiler.LeaveContext();
                                yield break;
                        }
                        compiler.SyntaxError(token, false, "err-compiler-unexpected-token", token.Value);
                        break;

                    case R.RightAngle:
                        compiler.SyntaxError(token, false, "err-compiler-unexpected-token", token.Value);
                        break;

                    // the end of a block weight, maybe
                    case R.RightParen:
                        if (context == CompileContext.BlockWeight)
                        {
                            reader.SkipSpace();
                            compiler.LeaveContext();
                            yield break;
                        }
                        actionCallback(new RstText(token.Value, token.ToLocation()));
                        break;

                    case R.Whitespace:
                        switch (context)
                        {
                            case CompileContext.BlockSequence:
                                switch (reader.PeekType())
                                {
                                    case R.Pipe:
                                    case R.RightCurly:
                                        continue; // Ignore whitespace at the end of block elements
                                }
                                goto default;
                            default:
                                actionCallback(new RstText(token.Value, token.ToLocation()));
                                break;
                        }
                        break;

                    case R.EscapeSequenceChar: // Handle escape sequences
                        actionCallback(new RstEscape(token.ToLocation(), 1, false, token.Value[0]));
                        break;
                    case R.EscapeSequenceUnicode:
                    {
                        short codePoint;
                        if (!short.TryParse(token.Value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out codePoint))
                        {
                            compiler.SyntaxError(reader.PrevToken, false, "err-compiler-invalid-escape-unicode", reader.PrevToken.Value);
                            break;
                        }
                        actionCallback(new RstEscape(token.ToLocation(), 1, true, Convert.ToChar(codePoint)));
                        break;
                    }
                    case R.EscapeSequenceSurrogatePair:
                    {
                        uint surrogatePairCodePoint;
                        if (!uint.TryParse(token.Value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out surrogatePairCodePoint)
                            || surrogatePairCodePoint < 0x10000)
                        {
                            compiler.SyntaxError(reader.PrevToken, false, "err-compiler-invalid-escape-surrogate", token.Value);
                            break;
                        }

                        surrogatePairCodePoint -= 0x10000;
                        ushort highCodePoint = (ushort)(0xD800 + ((surrogatePairCodePoint & 0xFFC00) >> 10));
                        ushort lowCodePoint = (ushort)(0xDC00 + (surrogatePairCodePoint & 0x003FF));
                        char low, high;
                        if (!char.IsSurrogatePair(high = Convert.ToChar(highCodePoint), low = Convert.ToChar(lowCodePoint)))
                        {
                            compiler.SyntaxError(reader.PrevToken, false, "err-compiler-invalid-escape-surrogate", token.Value);
                            break;
                        }
                        actionCallback(new RstEscape(token.ToLocation(), 1, true, high, low));
                        break;
                    }
                    case R.EscapeSequenceQuantifier:
                    {
                        int quantity;
                        if (!int.TryParse(token.Value, out quantity) || quantity <= 0)
                        {
                            compiler.SyntaxError(token, false, "err-compiler-escape-bad-quantity");
                            break;
                        }
                        switch (reader.PeekType())
                        {
                            case R.EscapeSequenceChar:
                                actionCallback(new RstEscape(token.ToLocation(), quantity, false, reader.ReadToken().Value[0]));
                                break;
                            case R.EscapeSequenceUnicode:
                            {
                                short codePoint;
                                if (!short.TryParse(reader.ReadToken().Value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out codePoint))
                                {
                                    compiler.SyntaxError(reader.PrevToken, false, "err-compiler-invalid-escape-unicode", reader.PrevToken.Value);
                                    break;
                                }
                                actionCallback(new RstEscape(token.ToLocation(), quantity, true, Convert.ToChar(codePoint)));
                                break;
                            }
                            case R.EscapeSequenceSurrogatePair:
                            {
                                var pairToken = reader.ReadToken();
                                uint surrogatePairCodePoint;
                                if (!uint.TryParse(pairToken.Value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out surrogatePairCodePoint)
                                    || surrogatePairCodePoint < 0x10000)
                                {
                                    compiler.SyntaxError(reader.PrevToken, false, "err-compiler-invalid-escape-surrogate", pairToken.Value);
                                    break;
                                }

                                surrogatePairCodePoint -= 0x10000;
                                ushort highCodePoint = (ushort)(0xD800 + ((surrogatePairCodePoint & 0xFFC00) >> 10));
                                ushort lowCodePoint = (ushort)(0xDC00 + (surrogatePairCodePoint & 0x3FF));
                                char low, high;
                                if (!char.IsSurrogatePair(high = Convert.ToChar(highCodePoint), low = Convert.ToChar(lowCodePoint)))
                                {
                                    compiler.SyntaxError(reader.PrevToken, false, "err-compiler-invalid-escape-surrogate", pairToken.Value);
                                    break;
                                }
                                actionCallback(new RstEscape(token.ToLocation(), quantity, true, high, low));
                                break;
                            }
                        }
                        break;
                    }

                    case R.EOF:
                        if (context != CompileContext.DefaultSequence)
                            compiler.SyntaxError(token, true, "err-compiler-eof");
                        yield break;

                    default: // Handle text
                        actionCallback(new RstText(token.Value, token.ToLocation()));
                        break;
                }
            }

            if (reader.End && context != CompileContext.DefaultSequence)
                compiler.SyntaxError(reader.PrevToken, true, "err-compiler-eof");
        }
    }
}