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

        public virtual void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}