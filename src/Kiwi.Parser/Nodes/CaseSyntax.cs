using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
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

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}