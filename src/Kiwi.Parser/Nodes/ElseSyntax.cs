using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ElseSyntax : ISyntaxBase
    {
        public ElseSyntax(IStatementSyntax statements)
        {
            Statements = statements;
        }

        public IStatementSyntax Statements { get; }

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