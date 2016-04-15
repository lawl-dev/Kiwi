using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class UsingSyntax : ISyntaxBase
    {
        public UsingSyntax(Token namespaceName)
        {
            NamespaceName = namespaceName;
        }

        public Token NamespaceName { get; private set; }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}