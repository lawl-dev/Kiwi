using System.Collections.Generic;

namespace Kiwi.Lexer.Strategies
{
    internal class SyntaxSymbolLexerStrategy : TokenLexerStrategyBase
    {
        private readonly List<string> _forbiddenSymbolNames;

        public SyntaxSymbolLexerStrategy(List<string> forbiddenSymbolNames)
        {
            _forbiddenSymbolNames = forbiddenSymbolNames;
        }

        public override Token GetToken(TransactableTokenStream stream)
        {
            stream.TakeSnapshot();

            if (stream.Current == null || char.IsNumber(stream.Current[0]) || !char.IsLetter(stream.Current[0]))
            {
                stream.RollbackSnapshot();
                return null;
            }

            var symbolName = string.Empty;
            while (stream.Current != null && char.IsLetter(stream.Current[0]))
            {
                symbolName += stream.Current[0];
                stream.Consume();
            }

            if (_forbiddenSymbolNames.Contains(symbolName))
            {
                stream.RollbackSnapshot();
                return null;
            }

            stream.CommitSnapshot();
            return new Token(TokenType.Symbol, symbolName);
        }
    }
}