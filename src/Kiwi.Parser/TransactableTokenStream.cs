using System.Collections.Generic;
using Kiwi.Common;
using Kiwi.Lexer;

namespace Kiwi.Parser
{
    internal sealed class TransactableTokenStream : TransactableStreamBase<Token>
    {
        public TransactableTokenStream(List<Token> tokens) : base(() => tokens)
        {
        }
    }
}