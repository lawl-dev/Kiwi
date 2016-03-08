using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class SimpleWhenStatementSyntax : IStatementSyntax
    {
        public List<WhenEntry> WhenEntries { get; private set; }

        public SimpleWhenStatementSyntax(List<WhenEntry> whenEntries)
        {
            WhenEntries = whenEntries;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}