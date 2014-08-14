namespace Manhood
{
    internal sealed class Token
    {
        public string Text { get; private set; }
        public TokenType Type { get; private set; }

        public Token(string text, TokenType type)
        {
            Text = text;
            Type = type;
        }
    }
}