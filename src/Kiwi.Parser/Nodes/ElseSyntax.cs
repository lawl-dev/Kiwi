using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ElseSyntax : ISyntaxBase
    {
        public ElseSyntax(List<IStatementSyntax> statements)
        {
            Statements = statements;
        }

        public List<IStatementSyntax> Statements { get; }
        public SyntaxType SyntaxType => SyntaxType.ElseSyntax;

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