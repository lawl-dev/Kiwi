using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ConditionalWhenStatementSyntax : IStatementSyntax
    {
        public IExpressionSyntax Condition { get; private set; }
        public List<ConditionalWhenEntry> WhenEntries { get; private set; }

        public ConditionalWhenStatementSyntax(IExpressionSyntax condition, List<ConditionalWhenEntry> whenEntries)
        {
            Condition = condition;
            WhenEntries = whenEntries;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}