namespace Kiwi.Lexer.Strategies
{
    internal class SyntaxIntegerLexerStrategy : TokenLexerStrategyBase
    {
        protected static string LexInteger(TransactableTokenStream stream)
        {
            var number = string.Empty;
            while (stream.Current != null && char.IsNumber(stream.Current[0]))
            {
                number += stream.Current;
                stream.Consume();
            }
            return number;
        }

        public override Token GetToken(TransactableTokenStream stream)
        {
            stream.TakeSnapshot();
            
            var number = LexInteger(stream);
            if (number == string.Empty)
            {
                stream.RollbackSnapshot();
                return null;
            }

            return new Token(TokenType.Int, number);
        }
    }
}