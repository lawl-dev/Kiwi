using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class IfStatementSyntax : IStatementSyntax
    {
        public List<IExpressionSyntax> Condition { get; }
        public List<IStatementSyntax> Statements { get; } 

        public IfStatementSyntax(List<IExpressionSyntax> condition, List<IStatementSyntax> statements)
        {
            Condition = condition;
            Statements = statements;
        }

        public virtual void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}