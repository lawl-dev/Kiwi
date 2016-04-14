using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ArrayCreationExpressionSyntax : ObjectCreationExpressionSyntax
    {
        public ArrayCreationExpressionSyntax(TypeSyntax type, List<IExpressionSyntax> parameter) : base(type, parameter)
        {
        }

        public int Dimension => Parameter.Count;
        public override SyntaxType SyntaxType => SyntaxType.ArrayCreationExpression;

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}