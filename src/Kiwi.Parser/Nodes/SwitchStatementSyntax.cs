using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class SwitchStatementSyntax : IStatetementSyntax
    {
        public IExpressionSyntax Condition { get; }
        public List<CaseSyntax> Cases { get; }
        public DefaultSyntax Default { get; }

        public SwitchStatementSyntax(IExpressionSyntax condition, List<CaseSyntax> cases, DefaultSyntax @default)
        {
            Condition = condition;
            Cases = cases;
            Default = @default;
        }
    }
}