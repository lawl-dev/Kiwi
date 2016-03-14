using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ArrayCreationExpressionSyntax : ObjectCreationExpressionSyntax
    {
        public int Dimension => Parameter.Count;

        public ArrayCreationExpressionSyntax(TypeSyntax type, List<IExpressionSyntax> parameter) : base(type, parameter)
        {
        }
        
        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}