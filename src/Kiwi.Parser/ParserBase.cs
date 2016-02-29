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

        protected List<ISyntaxBase> ParseInnerCommmaSeperated(
            TokenType opening,
            TokenType closing,
            Func<ISyntaxBase> parser)
        {
            return ParseInner(opening, closing, parser, true);
        }

        protected List<ISyntaxBase> ParseInner(TokenType opening, TokenType closing, Func<ISyntaxBase> parser, bool commaSeperated = false)
        {
            var innerSyntax = new List<ISyntaxBase>();
            Consume(opening);
            while (TokenStream.Current.Type != closing)
            {
                var inner = parser();
                if (inner == null)
                {
                    throw new KiwiSyntaxException("Unexpected Syntax in Sope");
                }

                innerSyntax.Add(inner);
                if (TokenStream.Current.Type != closing && commaSeperated)
                {
                    Consume(TokenType.Comma);
                }
            }

            Consume(closing);
            return innerSyntax;
        }

        protected ISyntaxBase Parse(params Func<ISyntaxBase>[] possibleParseFunctions)
        {
            foreach (var parseFunction in possibleParseFunctions)
            {
                TokenStream.TakeSnapshot();
                var result = parseFunction();
                if (result == null)
                {
                    TokenStream.RollbackSnapshot();
                }
                else
                {
                    TokenStream.CommitSnapshot();
                    return result;
                }
            }
            return null;
        }

        protected Token Consume(TokenType tokenType)
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