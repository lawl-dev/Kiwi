using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ConstructorSyntax : ISyntaxBase
    {
        public ConstructorSyntax(List<ParameterSyntax> argList, List<IStatementSyntax> statements)
        {
            ArgList = argList;
            Statements = statements;
        }

        public List<ParameterSyntax> ArgList { get; private set; }
        public List<IStatementSyntax> Statements { get; }

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