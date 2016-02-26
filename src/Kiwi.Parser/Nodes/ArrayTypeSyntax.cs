using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ArrayTypeSyntax : TypeSyntax
    {
        public int Dimension { get; }

        public ArrayTypeSyntax(Token typeName, int dimension) : base(typeName)
        {
            Dimension = dimension;
        }
    }
}