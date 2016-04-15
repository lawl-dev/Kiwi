using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ImplicitParameterTypeAnonymousFunctionExpressionSyntax : IExpressionSyntax
    {
        public ImplicitParameterTypeAnonymousFunctionExpressionSyntax(
            List<IExpressionSyntax> parameter,
            List<IStatementSyntax> statements)
        {
            Parameter = parameter;
            Statements = statements;
        }

        public List<IExpressionSyntax> Parameter { get; }
        public List<IStatementSyntax> Statements { get; }

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