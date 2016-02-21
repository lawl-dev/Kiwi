namespace Kiwi.Lexer.Strategies
{
    internal class SyntaxFloatLexerStrategy : SyntaxIntegerLexerStrategy
    {
        public override Token GetToken(TransactableTokenStream stream)
        {
            stream.TakeSnapshot();
            var leftPart = LexInteger(stream);

            var postfix = GetPostfix(stream);
            if (postfix != null)
            {
                return new Token(TokenType.Float, $"{leftPart}{postfix}");
            }

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

            postfix = GetPostfix(stream) ?? string.Empty;

            stream.CommitSnapshot();
            return new Token(TokenType.Float, $"{leftPart}.{rightPart}{postfix}");
        }

        private static string GetPostfix(TransactableTokenStream stream)
        {
            if (stream.Current == "f" || stream.Current == "F")
            {
                var postfix = stream.Current;
                stream.Consume();
                return postfix;
            }
            return null;
        }
    }
}