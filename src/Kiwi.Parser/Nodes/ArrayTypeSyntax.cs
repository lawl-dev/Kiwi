using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ArrayTypeSyntax : TypeSyntax
    {
        public int Dimension { get; }
        public List<IExpressionSyntax> Sizes { get; }

        public ArrayTypeSyntax(Token typeName, int dimension, List<IExpressionSyntax> sizes) : base(typeName)
        {
            Dimension = dimension;
            Sizes = sizes;
        }
    }
}