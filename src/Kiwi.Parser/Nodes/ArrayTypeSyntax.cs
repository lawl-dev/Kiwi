using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ArrayTypeSyntax : TypeSyntax
    {
        public ArrayTypeSyntax(Token typeName, int dimension) : base(typeName)
        {
            Dimension = dimension;
        }

        public int Dimension { get; }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}