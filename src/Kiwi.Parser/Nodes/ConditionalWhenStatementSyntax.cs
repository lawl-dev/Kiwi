using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ConditionalWhenStatementSyntax : SimpleWhenStatementSyntax
    {
        public ConditionalWhenStatementSyntax(List<WhenEntry> whenEntries) : base(whenEntries)
        {
        }
    }
}