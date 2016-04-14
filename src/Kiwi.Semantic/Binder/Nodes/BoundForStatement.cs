using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundForStatement : BoundStatement
    {
        public BoundForStatement(
            BoundStatement initStatement,
            BoundExpression condition,
            BoundStatement loopStatement,
            List<BoundStatement> boundStatements,
            ForStatementSyntax statementSyntax) : base(statementSyntax)
        {
            InitStatement = initStatement;
            Condition = condition;
            LoopStatement = loopStatement;
            BoundStatements = boundStatements;
        }

        public BoundStatement InitStatement { get; set; }
        public BoundExpression Condition { get; set; }
        public BoundStatement LoopStatement { get; set; }
        public List<BoundStatement> BoundStatements { get; set; }
    }
}