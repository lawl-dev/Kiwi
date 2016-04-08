using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ArrayAccessExpression : IExpressionSyntax
    {
        public IExpressionSyntax Owner { get; }
        public List<IExpressionSyntax> Parameter { get; }
        public SyntaxType SyntaxType => SyntaxType.ArrayAccessExpression;

        public ArrayAccessExpression(IExpressionSyntax owner, List<IExpressionSyntax> parameter)
        {
            Owner = owner;
            Parameter = parameter;
        }
        
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