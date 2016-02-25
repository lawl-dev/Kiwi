using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class SwitchStatementSyntax : IStatetementSyntax
    {
        public IExpressionSyntax Condition { get; }
        public IEnumerable<CaseSyntax> Cases { get; }
        public DefaultSyntax Default { get; }

        public SwitchStatementSyntax(IExpressionSyntax condition, IEnumerable<CaseSyntax> cases, DefaultSyntax @default)
        {
            Condition = condition;
            Cases = cases;
            Default = @default;
        }
    }
}