using System.Collections.Generic;
using System.Linq;

namespace Kiwi.Parser.Nodes
{
    public class ConstructorSyntax : ISyntaxBase
    {
        public List<ParameterSyntax> ArgList { get; private set; }
        public List<IStatementSyntax> Statements { get; }

        public ConstructorSyntax(List<ParameterSyntax> argList, List<IStatementSyntax> statements)
        {
            ArgList = argList;
            Statements = statements;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}