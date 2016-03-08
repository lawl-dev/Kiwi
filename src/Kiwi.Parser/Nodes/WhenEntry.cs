using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class WhenEntry : ISyntaxBase
    {
        public IExpressionSyntax Condition { get; set; }
        public List<IStatementSyntax> Statements { get; set; }

        public WhenEntry(IExpressionSyntax condition, List<IStatementSyntax> statements)
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