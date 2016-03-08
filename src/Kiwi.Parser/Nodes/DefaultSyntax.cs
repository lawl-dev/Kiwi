using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class DefaultSyntax : ISyntaxBase
    {
        public List<IStatementSyntax> Statements { get; }

        public DefaultSyntax(List<IStatementSyntax> statements)
        {
            Statements = statements;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}