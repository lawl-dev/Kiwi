using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ForInStatementSyntax : IStatementSyntax
    {
        public ForInStatementSyntax(
            VariableDeclarationStatementSyntax variableDeclarationStatement,
            IExpressionSyntax collExpression,
            List<IStatementSyntax> statements)
        {
            VariableDeclarationStatement = variableDeclarationStatement;
            CollExpression = collExpression;
            Statements = statements;
        }

        public ForInStatementSyntax(
            IExpressionSyntax itemExpression,
            IExpressionSyntax collExpression,
            List<IStatementSyntax> statements)
        {
            ItemExpression = itemExpression;
            CollExpression = collExpression;
            Statements = statements;
        }

        public VariableDeclarationStatementSyntax VariableDeclarationStatement { get; }
        public IExpressionSyntax ItemExpression { get; }
        public IExpressionSyntax CollExpression { get; }
        public List<IStatementSyntax> Statements { get; }
        public SyntaxType SyntaxType => SyntaxType.ForInStatementSyntax;

        public virtual void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}