using System.Collections.Generic;
using System.Linq;

using Manhood.Compiler;

using Stringes;
using Stringes.Tokens;

namespace Manhood.Blueprints
{
    internal sealed class TagBlueprint : Blueprint
    {
        public Source Source { get; private set; }
        public Stringe Name { get; private set; }

        private readonly Interpreter.TagDef _tagDef;

        private readonly TagArg[] _args;

        public TagBlueprint(Interpreter interpreter, Source source, Stringe name, IEnumerable<Token<TokenType>>[] args = null)
            : base(interpreter)
        {
            Source = source;
            Name = name;

            if (!Interpreter.TagFuncs.TryGetValue(Name.Value.ToLower().Trim(), out _tagDef))
            {
                throw new ManhoodException(Source, Name, "The tag '" + Name.Value + "' does not exist.");
            }

            _tagDef.ValidateArgCount(source, name, args != null ? args.Length : 0);

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
                        interpreter.PushState(Interpreter.State.CreateDerivedDistinct(source, args[i], interpreter));
                    }
                }
            }
        }

        public override bool Use()
        {
            // Fill in empty string arguments with state results.
            for (int i = 0; i < _args.Length; i++)
            {
                if (_args[i] == null) _args[i] = TagArg.FromString(I.PopResultString());
            }

            // Call the tag
            return _tagDef.Invoke(I, Source, Name, _args);
        }
    }
}