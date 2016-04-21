using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundSwitchStatement : BoundStatement
    {
        public BoundExpression BoundCondition { get; }
        public List<BoundCase> BoundCases { get; }
        public BoundElse BoundElse { get; }

        public BoundSwitchStatement(BoundExpression boundCondition, List<BoundCase> boundCases, BoundElse boundElse, SwitchStatementSyntax syntax) : base(syntax)
        {
            BoundCondition = boundCondition;
            BoundCases = boundCases;
            BoundElse = boundElse;
        }
    }
}