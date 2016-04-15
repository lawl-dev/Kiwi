using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class CaseSyntax : ISyntaxBase
    {
        public CaseSyntax(IExpressionSyntax expression, List<IStatementSyntax> statements)
        {
            Expression = expression;
            Statements = statements;
        }

        public IExpressionSyntax Expression { get; }
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