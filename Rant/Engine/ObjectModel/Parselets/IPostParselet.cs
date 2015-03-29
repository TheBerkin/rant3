using Rant.Engine.Compiler;
using Rant.Engine.ObjectModel.Expressions;
using Rant.Stringes;

namespace Rant.Engine.ObjectModel.Parselets
{
    /// <summary>
    /// Represents a parselet that handles infix and postfix operators.
    /// </summary>
    internal interface IPostParselet
    {
        /// <summary>
        /// Gets the precedence of the operation associated with the parselet. If it is greater than that of the previous operation, 
        /// </summary>
        int Precedence { get; }

        RantExpression Parse(RantState state, RantExpression left, Token<R> tokenOperator);
    }
}