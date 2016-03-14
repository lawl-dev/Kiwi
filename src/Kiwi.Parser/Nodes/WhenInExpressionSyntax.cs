using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class WhenInExpressionSyntax : IExpressionSyntax
    {
        public List<IExpressionSyntax> InExpressionList { get; }

        public WhenInExpressionSyntax(List<IExpressionSyntax> inExpressionList)
        {
            InExpressionList = inExpressionList;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}