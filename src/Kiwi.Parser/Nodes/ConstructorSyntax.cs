using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ConstructorSyntax : ISyntaxBase
    {
        public ConstructorSyntax(List<ParameterSyntax> parameter, ScopeStatementSyntax statements)
        {
            Parameter = parameter;
            Statements = statements;
        }

        public List<ParameterSyntax> Parameter { get; }
        public ScopeStatementSyntax Statements { get; }

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