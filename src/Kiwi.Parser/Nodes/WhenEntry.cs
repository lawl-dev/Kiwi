using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class WhenEntry : ISyntaxBase
    {
        public WhenEntry(IExpressionSyntax condition, IStatementSyntax statements)
        {
            Condition = condition;
            Statements = statements;
        }

        public IExpressionSyntax Condition { get; set; }
        public IStatementSyntax Statements { get; set; }

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