using Rant.Engine.Compiler;
using Rant.Engine.ObjectModel.Expressions;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
    /// <summary>
    /// Represents a parselet that handles prefix operators.
    /// </summary>
    internal interface IPreParselet
    {
        RantExpression Parse(VM.State state, Token<R> tokenPrefix);
    }
}