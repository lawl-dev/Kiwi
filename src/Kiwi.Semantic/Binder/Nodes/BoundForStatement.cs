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
            BoundScopeStatement boundStatements,
            ForStatementSyntax statementSyntax) : base(statementSyntax)
        {
            InitStatement = initStatement;
            Condition = condition;
            LoopStatement = loopStatement;
            BoundStatements = boundStatements;
        }

        public BoundStatement InitStatement { get; }
        public BoundExpression Condition { get; }
        public BoundStatement LoopStatement { get; }
        public BoundScopeStatement BoundStatements { get; }
    }
}