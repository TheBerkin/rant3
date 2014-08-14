namespace Manhood
{
    internal enum Precedence : int
    {
        Assignment,
        Sum,
        Product,
        Exponent,
        Prefix,
        Postfix
    }
}