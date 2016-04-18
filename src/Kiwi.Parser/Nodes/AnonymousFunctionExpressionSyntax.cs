using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class AnonymousFunctionExpressionSyntax : IExpressionSyntax
    {
        public AnonymousFunctionExpressionSyntax(List<ParameterSyntax> parameter, IStatementSyntax statements)
        {
            Parameter = parameter;
            Statements = statements;
        }

        public List<ParameterSyntax> Parameter { get; }
        public IStatementSyntax Statements { get; }
        
        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}