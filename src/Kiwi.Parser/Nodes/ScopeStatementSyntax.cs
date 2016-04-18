using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ScopeStatementSyntax : IStatementSyntax
    {
        public List<IStatementSyntax> Statements { get; }

        public ScopeStatementSyntax(List<IStatementSyntax> statements)
        {
            Statements = statements;
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