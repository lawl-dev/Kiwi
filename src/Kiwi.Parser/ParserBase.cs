using System;
using System.Collections.Generic;
using Kiwi.Common;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public abstract class ParserBase
    {
        protected readonly TransactableTokenStream TokenStream;

        protected ParserBase(TransactableTokenStream transactableTokenStream)
        {
            TokenStream = transactableTokenStream;
        }

        protected List<TSyntax> ParseInnerCommmaSeperated<TSyntax>(
            TokenType opening,
            TokenType closing,
            Func<TSyntax> parser) where TSyntax : ISyntaxBase
        {
            return ParseInner(opening, closing, parser, true);
        }

        protected List<TSyntax> ParseInner<TSyntax>(
            TokenType opening,
            TokenType closing,
            Func<TSyntax> parser,
            bool commaSeperated = false) where TSyntax: ISyntaxBase
        {
            var innerSyntax = new List<TSyntax>();
            ParseExpected(opening);
            while (TokenStream.Current.Type != closing)
            {
                var inner = parser();
                innerSyntax.Add(inner);
                if (TokenStream.Current.Type != closing && commaSeperated)
                {
                    ParseExpected(TokenType.Comma);
                }
            }

            ParseExpected(closing);
            return innerSyntax;
        }
        

        protected Token ParseExpected(TokenType tokenType)
        {
            var currentToken = TokenStream.Current;
            if (currentToken.Type != tokenType)
            {
                throw new KiwiSyntaxException($"Unexepted TokenType {currentToken.Type}. Excepted {tokenType}");
            }
            TokenStream.Consume();
            return currentToken;
        }
    }
}