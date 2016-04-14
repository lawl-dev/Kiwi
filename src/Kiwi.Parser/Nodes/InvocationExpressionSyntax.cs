using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class InvocationExpressionSyntax : IExpressionSyntax
    {
        public InvocationExpressionSyntax(IExpressionSyntax toInvoke, List<IExpressionSyntax> parameter)
        {
            ToInvoke = toInvoke;
            Parameter = parameter;
        }

        public IExpressionSyntax ToInvoke { get; }
        public List<IExpressionSyntax> Parameter { get; }
        public SyntaxType SyntaxType => SyntaxType.InvocationExpressionSyntax;

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