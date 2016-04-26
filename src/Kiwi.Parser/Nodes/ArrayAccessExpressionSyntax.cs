using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ArrayAccessExpressionSyntax : IExpressionSyntax
    {
        public ArrayAccessExpressionSyntax(IExpressionSyntax owner, List<IExpressionSyntax> parameter)
        {
            Owner = owner;
            Parameter = parameter;
        }

        public IExpressionSyntax Owner { get; }
        public List<IExpressionSyntax> Parameter { get; }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}