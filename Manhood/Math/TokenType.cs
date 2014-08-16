using System;

namespace Manhood
{
    internal enum TokenType : int
    {
        LeftParen, // (
        RightParen, // )
        Equals, // =
        Plus, // +
        Minus, // -
        Asterisk, // *
        Slash, // /
        Caret, // ^
        Name,
        Number,
        Increment,
        Decrement,
        Modulo,
        Swap,
        AddAssign,
        SubAssign,
        MulAssign,
        DivAssign,
        ModAssign,
        PowAssign,
        End
    }
}