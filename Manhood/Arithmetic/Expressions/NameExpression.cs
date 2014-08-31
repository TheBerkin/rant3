using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood.Arithmetic
{
    internal class NameExpression : Expression
    {
        private readonly Token<TokenType> _name;

        public NameExpression(Token<TokenType> token)
        {
            _name = token;
        }

        public string Name
        {
            get { return _name.Value; }
        }

        public override double Evaluate(Source source, Interpreter ii)
        {
            var d = ii.Engine.Variables.GetVar(_name.Value);
            if (d == null) throw new ManhoodException(source, _name, "Tried to access undefined variable '" + _name.Value + "'.");
            return d.Value;
        }
    }
}