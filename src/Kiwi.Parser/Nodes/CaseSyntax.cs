using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class CaseSyntax : ISyntaxBase
    {
        public IExpressionSyntax Expression { get; }
        public List<IStatementSyntax> Statements { get; }
        public SyntaxType SyntaxType => SyntaxType.CaseSyntax;

        public CaseSyntax(IExpressionSyntax expression, List<IStatementSyntax> statements)
        {
            Expression = expression;
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