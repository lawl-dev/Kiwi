using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class SimpleWhenStatementSyntax : IStatementSyntax
    {
        public List<WhenEntry> WhenEntries { get; private set; }
        public SyntaxType SyntaxType => SyntaxType.SimpleWhenStatementSyntax;

        public SimpleWhenStatementSyntax(List<WhenEntry> whenEntries)
        {
            WhenEntries = whenEntries;
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