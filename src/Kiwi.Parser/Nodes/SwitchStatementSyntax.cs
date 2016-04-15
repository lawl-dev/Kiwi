using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class SwitchStatementSyntax : IStatementSyntax
    {
        public SwitchStatementSyntax(IExpressionSyntax condition, List<CaseSyntax> cases, ElseSyntax @else)
        {
            Condition = condition;
            Cases = cases;
            Else = @else;
        }

        public IExpressionSyntax Condition { get; }
        public List<CaseSyntax> Cases { get; }
        public ElseSyntax Else { get; }

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