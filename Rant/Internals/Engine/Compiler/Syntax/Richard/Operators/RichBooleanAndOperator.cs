using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard.Operators
{
    internal class RichBooleanAndOperator : RichInfixOperator
    {
        public RichBooleanAndOperator(Stringe token)
            : base(token)
        {
            Type = ActionValueType.Boolean;
            Precedence = 15;
        }

        public override object GetValue(Sandbox sb)
        {
            var leftVal = sb.ScriptObjectStack.Pop();
            var rightVal = sb.ScriptObjectStack.Pop();

            if (!(leftVal is bool))
                throw new RantRuntimeException(sb.Pattern, Range, "Invalid left hand side of boolean AND operator.");
            if (!(rightVal is bool))
                throw new RantRuntimeException(sb.Pattern, Range, "Invalid right hand side of boolean AND operator.");

            return (bool)leftVal && (bool)rightVal;
        }
    }
}
