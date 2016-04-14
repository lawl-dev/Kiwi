using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class TypeSyntax : ISyntaxBase
    {
        public TypeSyntax(Token typeName)
        {
            TypeName = typeName;
        }

        public Token TypeName { get; }

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