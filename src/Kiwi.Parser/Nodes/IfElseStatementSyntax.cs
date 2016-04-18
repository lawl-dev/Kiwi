using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class IfElseStatementSyntax : IfStatementSyntax
    {
        public IfElseStatementSyntax(
            IExpressionSyntax condition,
            ScopeStatementSyntax statements,
            ScopeStatementSyntax elseStatements) : base(condition, statements)
        {
            ElseStatements = elseStatements;
        }

        public ScopeStatementSyntax ElseStatements { get; }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}