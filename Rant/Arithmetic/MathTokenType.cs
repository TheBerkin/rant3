namespace Rant.Arithmetic
{
    internal enum MathTokenType : int
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