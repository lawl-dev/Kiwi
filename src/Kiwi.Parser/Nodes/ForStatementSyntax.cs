using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ForStatementSyntax : IStatementSyntax
    {
        public ForStatementSyntax(
            IStatementSyntax initStatement,
            IExpressionSyntax condExpression,
            IStatementSyntax loopStatement,
            ScopeStatementSyntax statements)
        {
            InitStatement = initStatement;
            CondExpression = condExpression;
            LoopStatement = loopStatement;
            Statements = statements;
        }

        public IStatementSyntax InitStatement { get; }
        public IExpressionSyntax CondExpression { get; }
        public IStatementSyntax LoopStatement { get; }
        public ScopeStatementSyntax Statements { get; }

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