namespace Kiwi.Lexer
{
    public class Token
    {
        public Token(TokenType tokenType, string value)
        {
            Type = tokenType;
            Value = value;
        }

        public TokenType Type { get; }
        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}