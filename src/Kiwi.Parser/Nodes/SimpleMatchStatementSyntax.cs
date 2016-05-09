using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class SimpleMatchStatementSyntax : IStatementSyntax
    {
        public SimpleMatchStatementSyntax(List<WhenEntry> whenEntries)
        {
            WhenEntries = whenEntries;
        }

        public List<WhenEntry> WhenEntries { get; }

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