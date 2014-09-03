using System;
using System.Collections.Generic;
using System.Linq;

using Manhood.Compiler;

using Stringes;
using Stringes.Tokens;

namespace Manhood
{
    internal partial class Interpreter
    {
		/// <summary>
		/// Represents an outline for an action that is queued by a state object and executed immediately before parsing any tokens in the state.
		/// </summary>
        internal abstract class Blueprint
        {
            protected readonly Interpreter I;

            protected Blueprint(Interpreter interpreter)
            {
	            I = interpreter;
            }

            public abstract bool Use();
        }

        internal sealed class TagBlueprint : Blueprint
        {
            public Source Source { get; private set; }
            public Stringe Name { get; private set; }

            private readonly TagDef _tagDef;

            private readonly TagArg[] _args; 

            public TagBlueprint(Interpreter interpreter, Source source, Stringe name, IEnumerable<Token<TokenType>>[] args = null) : base(interpreter)
            {
                Source = source;
                Name = name;

                if (!TagFuncs.TryGetValue(Name.Value.ToLower(), out _tagDef))
                {
                    throw new ManhoodException(Source, Name, "The tag '" + Name.Value + "' does not exist.");
                }

                if (args == null)
                {
                    _args = Enumerable.Empty<TagArg>().ToArray();
                }
                else
                {
                    // Insert token arguments into the array, set string args to null.
                    _args = args.Select((a, i) => _tagDef.ArgTypes[i] == TagArgType.Tokens ? TagArg.FromTokens(a) : null).ToArray();

                    // Queue string arguments on the stack.
                    for (int i = 0; i < _tagDef.ArgTypes.Length; i++)
                    {
                        if (_tagDef.ArgTypes[i] == TagArgType.Result)
                        {
                            interpreter.PushState(State.CreateDerivedDistinct(source, args[i], interpreter));
                        }
                    }
                }
            }

            public override bool Use()
            {
                // Fill in empty string arguments with state results.
                for (int i = 0; i < _args.Length; i++)
                {
                    if(_args[i] == null) _args[i] = TagArg.FromString(I._resultStack.Pop().MainOutput);
                }

                // Call the tag
                _tagDef.Invoke(I, Source, Name, _args);

                return false;
            }
        }

        internal sealed class RepeaterBlueprint : Blueprint
        {
            private readonly Repeater _repeater;

            public RepeaterBlueprint(Interpreter interpreter, Repeater repeater) : base(interpreter)
            {
                _repeater = repeater;
            }

            // TODO: Add support for synchronizers
            // TODO: Consider storing repeaters in a public stack to allow access to [first], [last], etc.
            public override bool Use()
            {
                return _repeater.Iterate(I, this);
            }
        }

        internal sealed class MetapatternBlueprint : Blueprint
        {
            public MetapatternBlueprint(Interpreter interpreter) : base(interpreter)
            {
            }

            public override bool Use()
            {
                var srcstr = I.PopResult();
                var src = new Source("Meta_" + String.Format("{0:X16}", srcstr.Hash()), SourceType.Metapattern, srcstr);
                I.PushState(State.Create(src, I));
                return true;
            }
        }
    }
}