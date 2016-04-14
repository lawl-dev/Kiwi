using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class WhenInExpressionSyntax : IExpressionSyntax
    {
        public WhenInExpressionSyntax(List<IExpressionSyntax> inExpressionList)
        {
            InExpressionList = inExpressionList;
        }

        public List<IExpressionSyntax> InExpressionList { get; }
        public SyntaxType SyntaxType => SyntaxType.WhenInExpressionSyntax;

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