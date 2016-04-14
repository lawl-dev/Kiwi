using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ForStatementSyntax : IStatementSyntax
    {
        public ForStatementSyntax(
            IStatementSyntax initStatement,
            IExpressionSyntax condExpression,
            IStatementSyntax loopStatement,
            List<IStatementSyntax> statements)
        {
            InitStatement = initStatement;
            CondExpression = condExpression;
            LoopStatement = loopStatement;
            Statements = statements;
        }

        public IStatementSyntax InitStatement { get; }
        public IExpressionSyntax CondExpression { get; }
        public IStatementSyntax LoopStatement { get; }
        public List<IStatementSyntax> Statements { get; }
        public SyntaxType SyntaxType => SyntaxType.ForStatementSyntax;

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