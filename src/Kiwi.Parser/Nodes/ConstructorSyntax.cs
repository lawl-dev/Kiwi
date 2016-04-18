using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ConstructorSyntax : ISyntaxBase
    {
        public ConstructorSyntax(List<ParameterSyntax> argList, ScopeStatementSyntax statements)
        {
            ArgList = argList;
            Statements = statements;
        }

        public List<ParameterSyntax> ArgList { get; private set; }
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