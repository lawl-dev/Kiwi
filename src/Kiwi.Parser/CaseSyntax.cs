using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class CaseSyntax : ISyntaxBase
    {
        public IExpressionSyntax Expression { get; }
        public List<ISyntaxBase> Body { get; }

        public CaseSyntax(IExpressionSyntax expression, List<ISyntaxBase> body)
        {
            Expression = expression;
            Body = body;
        }
    }
}