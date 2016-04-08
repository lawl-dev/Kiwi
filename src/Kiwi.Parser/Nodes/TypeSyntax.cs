using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class TypeSyntax : ISyntaxBase
    {
        public Token TypeName { get; }

        public TypeSyntax(Token typeName)
        {
            TypeName = typeName;
        }

        public SyntaxType SyntaxType => SyntaxType.TypeSyntax;

        public virtual void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}