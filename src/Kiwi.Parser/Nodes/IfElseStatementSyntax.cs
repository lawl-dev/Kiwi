using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class IfElseStatementSyntax : IfStatementSyntax
    {
        public List<IStatementSyntax> ElseStatements { get; }

        public IfElseStatementSyntax(
            List<IExpressionSyntax> condition,
            List<IStatementSyntax> statements,
            List<IStatementSyntax> elseStatements) : base(condition, statements)
        {
            ElseStatements = elseStatements;
        }
    }
}