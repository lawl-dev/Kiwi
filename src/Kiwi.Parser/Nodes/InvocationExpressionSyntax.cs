using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class InvocationExpressionSyntax : IExpressionSyntax
    {
        public IExpressionSyntax ToInvoke { get; }
        public List<IExpressionSyntax> Parameter { get; }
        public SyntaxType SyntaxType => SyntaxType.InvocationExpressionSyntax;

        public InvocationExpressionSyntax(IExpressionSyntax toInvoke, List<IExpressionSyntax> parameter)
        {
            ToInvoke = toInvoke;
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