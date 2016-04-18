using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class IfStatementSyntax : IStatementSyntax
    {
        public IfStatementSyntax(IExpressionSyntax condition, ScopeStatementSyntax statements)
        {
            Condition = condition;
            Statements = statements;
        }

        public IExpressionSyntax Condition { get; }
        public ScopeStatementSyntax Statements { get; }

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