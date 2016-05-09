using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ConditionalMatchStatementSyntax : IStatementSyntax
    {
        public ConditionalMatchStatementSyntax(IExpressionSyntax condition, List<ConditionalWhenEntry> whenEntries)
        {
            Condition = condition;
            WhenEntries = whenEntries;
        }

        public IExpressionSyntax Condition { get; }
        public List<ConditionalWhenEntry> WhenEntries { get; }

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