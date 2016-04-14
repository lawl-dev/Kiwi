using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ObjectCreationExpressionSyntax : IExpressionSyntax
    {
        public ObjectCreationExpressionSyntax(TypeSyntax type, List<IExpressionSyntax> parameter)
        {
            Type = type;
            Parameter = parameter;
        }

        public TypeSyntax Type { get; }
        public List<IExpressionSyntax> Parameter { get; }
        public virtual SyntaxType SyntaxType => SyntaxType.ObjectCreationExpressionSyntax;

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