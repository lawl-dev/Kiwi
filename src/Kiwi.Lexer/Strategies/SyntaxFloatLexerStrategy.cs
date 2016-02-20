namespace Kiwi.Lexer.Strategies
{
    internal class SyntaxFloatLexerStrategy : SyntaxIntegerLexerStrategy
    {
        public override Token GetToken(TransactableTokenStream stream)
        {
            stream.TakeSnapshot();
            var leftPart = LexInteger(stream);

            if (stream.Current != ".")
            {
                stream.RollbackSnapshot();
                return null;
            }

            stream.Consume();
            var rightPart = LexInteger(stream);

            if (string.IsNullOrEmpty(rightPart))
            {
                stream.RollbackSnapshot();
                return null;
            }

            stream.CommitSnapshot();
            return new Token(TokenType.Float, leftPart + "." + rightPart);
        }
    }
}