using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ReverseForInStatementSyntax : ForInStatementSyntax
    {
        public ReverseForInStatementSyntax(
            VariableDeclarationStatementSyntax variableDeclarationStatement,
            IExpressionSyntax collExpression,
            IStatementSyntax statements) : base(variableDeclarationStatement, collExpression, statements)
        {
        }

        public ReverseForInStatementSyntax(
            IExpressionSyntax itemExpression,
            IExpressionSyntax collExpression,
            IStatementSyntax statements) : base(itemExpression, collExpression, statements)
        {
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}