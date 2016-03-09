using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ElseSyntax : ISyntaxBase
    {
        public List<IStatementSyntax> Statements { get; }

        public ElseSyntax(List<IStatementSyntax> statements)
        {
            Statements = statements;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}