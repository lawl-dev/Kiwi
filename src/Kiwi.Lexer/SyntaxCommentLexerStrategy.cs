using Kiwi.Lexer.Strategies;

namespace Kiwi.Lexer
{
    internal class SyntaxCommentLexerStrategy : TokenLexerStrategyBase
    {
        public override Token GetToken(TransactableTokenStream stream)
        {
            if (stream.Current != "/")
            {
                return null;
            }

            stream.TakeSnapshot();
            stream.Consume();
            string comment;
            switch (stream.Current)
            {
                case "/":
                    stream.Consume();
                    comment = LexSingleLineComment(stream);
                    break;
                case "*":
                    stream.Consume();
                    comment = LexMultiLineComment(stream);
                    break;
                default:
                    stream.RollbackSnapshot();
                    return null;
            }

            stream.CommitSnapshot();
            return new Token(TokenType.Comment, comment);
        }

        private static string LexMultiLineComment(TransactableTokenStream stream)
        {
            var comment = string.Empty;
            while (stream.Current != null && (stream.Current != "*" && stream.Peek(1) != "/"))
            {
                comment += stream.Current;
                stream.Consume();
            }
            if (stream.Current == "*" && stream.Peek(1) == "/")
            {
                comment += stream.Current;
                stream.Consume();
                comment += stream.Current;
                stream.Consume();
            }
            return $"/*{comment}";
        }

        private static string LexSingleLineComment(TransactableTokenStream stream)
        {
            var comment = string.Empty;
            while (stream.Current != null && (stream.Current != "\r" && stream.Peek(1) != "\n"))
            {
                comment += stream.Current;
                stream.Consume();
            }

            return $"//{comment}";
        }
    }
}