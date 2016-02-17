namespace Kiwi.Lexer.Strategies
{
    internal class SyntaxLexerStrategy : TokenLexerStrategyBase
    {
        private readonly TokenType _keyword;
        private readonly string _syntaxLiteral;

        public SyntaxLexerStrategy(TokenType keyword, string syntaxLiteral)
        {
            _keyword = keyword;
            _syntaxLiteral = syntaxLiteral;
        }

        public override Token GetToken(TransactableTokenStream stream)
        {
            stream.TakeSnapshot();
            
            foreach (var character in _syntaxLiteral)
            {
                if (stream.Current == null || stream.Current != character.ToString())
                {
                    stream.RollbackSnapshot();
                    return null;
                }
                stream.Consume();
            }

            stream.CommitSnapshot();
            return new Token(_keyword, _syntaxLiteral);
        }
    }
}