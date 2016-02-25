using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
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