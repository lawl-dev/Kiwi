namespace Kiwi.Lexer.Strategies
{
    internal sealed class SyntaxStringLexerStrategy : TokenLexerStrategyBase
    {
        public override Token GetToken(TransactableTokenStream stream)
        {
            if (stream.Current != "\"")
            {
                return null;
            }

            stream.TakeSnapshot();

            var tokenValue = stream.Current;
            stream.Consume();

            while (stream.Current != null && stream.Current != "\"")
            {
                tokenValue += stream.Current;
                stream.Consume();

                if (stream.Current == null)
                {
                    stream.RollbackSnapshot();
                    return null;
                }
            }
            
            tokenValue += stream.Current;
            stream.Consume();

            stream.CommitSnapshot();
            return new Token(TokenType.String, tokenValue);
        }
    }
}