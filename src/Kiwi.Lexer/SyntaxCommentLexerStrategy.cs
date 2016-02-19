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
                    comment = LexSingleLineComment(stream);
                    break;
                case "*":
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
            while (stream.Current != null && stream.Current != "*/")
            {
                comment += stream.Current;
                stream.Consume();
            }
            if (stream.Current == "*/")
            {
                comment += stream.Current;
                stream.Consume();
            }
            return comment;
        }

        private static string LexSingleLineComment(TransactableTokenStream stream)
        {
            var comment = string.Empty;
            while (stream.Current != null && stream.Current != "\r\n")
            {
                comment += stream.Current;
                stream.Consume();
            }
            return comment;
        }
    }
}