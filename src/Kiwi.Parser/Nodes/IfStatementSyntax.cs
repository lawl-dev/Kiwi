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

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}