using System.Collections.Generic;
using System.IO;

namespace Kiwi.Parser.Nodes
{
    public class AnonymousFunctionExpressionSyntax : IExpressionSyntax
    {
        public List<ParameterSyntax> Parameter { get; }
        public List<IStatementSyntax> Statements { get; }
        public SyntaxType SyntaxType => Nodes.SyntaxType.AnonymousFunctionExpressionSyntax;


        public AnonymousFunctionExpressionSyntax(List<ParameterSyntax> parameter, List<IStatementSyntax> statements)
        {
            Parameter = parameter;
            Statements = statements;
        }

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