using System.Collections.Generic;
using System.Linq;

namespace Kiwi.Parser.Nodes
{
    public class ConstructorSyntax : ISyntaxBase
    {
        public List<ParameterSyntax> ArgList { get; private set; }
        public List<IStatementSyntax> Statements { get; }
        public SyntaxType SyntaxType => SyntaxType.ConstructorSyntax;

        public ConstructorSyntax(List<ParameterSyntax> argList, List<IStatementSyntax> statements)
        {
            ArgList = argList;
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