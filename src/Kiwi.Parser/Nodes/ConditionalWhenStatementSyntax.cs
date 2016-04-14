using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ConditionalWhenStatementSyntax : IStatementSyntax
    {
        public ConditionalWhenStatementSyntax(IExpressionSyntax condition, List<ConditionalWhenEntry> whenEntries)
        {
            Condition = condition;
            WhenEntries = whenEntries;
        }

        public IExpressionSyntax Condition { get; private set; }
        public List<ConditionalWhenEntry> WhenEntries { get; private set; }
        public SyntaxType SyntaxType => SyntaxType.ConditionalWhenStatementSyntax;

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