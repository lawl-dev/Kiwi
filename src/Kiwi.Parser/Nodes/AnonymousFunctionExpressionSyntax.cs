using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class AnonymousFunctionExpressionSyntax : IExpressionSyntax
    {
        public List<ParameterSyntax> Parameter { get; }
        public List<IStatementSyntax> Statements { get; }

        public AnonymousFunctionExpressionSyntax(List<ParameterSyntax> parameter, List<IStatementSyntax> statements)
        {
            Parameter = parameter;
            Statements = statements;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}