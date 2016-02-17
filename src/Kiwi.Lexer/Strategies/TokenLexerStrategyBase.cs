namespace Kiwi.Lexer.Strategies
{
    internal abstract class TokenLexerStrategyBase
    {
        public bool IsMatch(TransactableTokenStream stream)
        {
            stream.TakeSnapshot();

            var isMatch = GetToken(stream) != null;

            stream.RollbackSnapshot();
            return isMatch;
        }

        public abstract Token GetToken(TransactableTokenStream stream);
    }
}